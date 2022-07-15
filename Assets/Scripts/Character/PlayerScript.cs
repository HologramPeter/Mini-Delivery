using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//interface to act for GM to interact with Model
public class PlayerScript : MonoBehaviour
{
    [SerializeField] public float mouseSensitivity = 200f;
    [SerializeField] public float speed = 3f;
    [SerializeField] float sprintMult = 2.5f;

    [SerializeField] float interactRadius = 5f;
    [SerializeField] LayerMask interactMask;

    [SerializeField] Camera cam;
    [SerializeField] GameObject model;

    [SerializeField] GameObject minimapLinePrefab;

    [Header("Music and sound")]
    [SerializeField] AudioClip WalkingSound;

    [SerializeField] AudioClip SpraySound;
    [SerializeField] AudioClip MaskSound;

    [SerializeField] AudioClip MotorStartSound;
    [SerializeField] AudioClip MotorEndSound;

    [SerializeField] AudioClip DieSound;

    [SerializeField] AudioClip CollectSound;
    [SerializeField] AudioClip DeliverSound;

    ModelHandler modelHandler;
    private LineRenderer line;
    Collider _collider;
    Rigidbody rb;

    Animator animator;
    float xRotation = 0f;
    bool isWalking = false;
    bool isRiding = false;
    bool isWearing = false;

    static List<int> orderTypes = new List<int>();
    static List<GameObject> minimapLines = new List<GameObject>();
    public static bool isPaused = false;

    AudioSource effect_audio;
    AudioSource movement_audio;



    public void Pause()
    {
        isPaused = true;
        movement_audio.Pause();
    }

    public void Resume()
    {
        isPaused = false;
        if (isWalking) movement_audio.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = model.GetComponent<Animator>();
        modelHandler = model.GetComponent<ModelHandler>();
        _collider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = 2;

        effect_audio = gameObject.AddComponent<AudioSource>();
        movement_audio = gameObject.AddComponent<AudioSource>();
        movement_audio.loop = true;
        movement_audio.clip = WalkingSound;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused) { rb.velocity = new Vector3(); return; }
        Move();
        RotateCamera();
        Interact();
    }

    #region movement

    private void Move()
    {
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        movement = movement.normalized * speed * (isRiding ? sprintMult : 1);
        movement = transform.forward * movement.z + transform.right * movement.x;

        if (movement.magnitude != 0)
            model.transform.forward = movement;
        rb.velocity = movement;

        if (movement.magnitude > 0.01f)
        {
            if (!isWalking)
            {
                isWalking = !isWalking;
                animator.SetBool("IsWalking", isWalking);
                movement_audio.Play();
            }
        }
        else
        {
            if (isWalking)
            {
                isWalking = !isWalking;
                animator.SetBool("IsWalking", isWalking);
                movement_audio.Stop();
            }
        }
    }

    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, 5f, 35f);

        transform.Rotate(Vector3.up * mouseX);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
    #endregion

    #region order

    void Interact()
    {
        GameObject closestBuilding = GetClosestBuilding();
        if (closestBuilding == null)
        {
            line.enabled = false;
            return;
        }

        line.enabled = true;
        DrawLineToBuilding(closestBuilding);

        if (!Input.GetKeyDown(KeyCode.Space)) return;

        if (closestBuilding.tag == "Restaurant")
        {
            (int orderType, GameObject targetHouse) = closestBuilding.GetComponent<RestaurantScript>().CollectOrder();
            if (orderType == -1) return;
            orderTypes.Add(orderType);
            GameObject minimapLine = Instantiate(minimapLinePrefab);
            minimapLine.GetComponent<MinimapLine>().SetLine(gameObject, targetHouse);
            minimapLines.Add(minimapLine);

            effect_audio.PlayOneShot(CollectSound);

            updateModelBag();
        }
        else
        {
            int index = closestBuilding.GetComponent<HouseScript>().DeliverOrder(orderTypes);
            if (index != -1)
            {
                Debug.Log("delete index " + index + "minimapLines:" + minimapLines.Count);
                Destroy(minimapLines[index]);
                minimapLines.RemoveAt(index);
                updateModelBag();
            }

            effect_audio.PlayOneShot(DeliverSound);
        }
    }

    void updateModelBag()
    {
        if (orderTypes.Count == 0)
        {
            modelHandler.SetBagActive(false);
        }
        else
        {
            modelHandler.SetBagActive(true);
        }
    }

    private void DrawLineToBuilding(GameObject building)
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, building.transform.position);
    }

    GameObject GetClosestBuilding()
    {
        Collider[] buildings_c = Physics.OverlapSphere(transform.position, interactRadius, interactMask);
        if (buildings_c.Length > 0)
        {
            GameObject closestBuilding = null;
            float minDistance = Mathf.Infinity;
            foreach (Collider building_c in buildings_c)
            {
                if (!building_c.GetComponent<BuildingScript>().isActive) continue;

                GameObject building = building_c.gameObject;
                float distance = (building.transform.position - transform.position).magnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestBuilding = building;
                }
            }
            return closestBuilding;
        }
        else return null;
    }

    public void RemoveOrder(int instanceID)
    {
        int index = orderTypes.IndexOf(instanceID);
        if (index == -1) return;

        orderTypes.RemoveAt(index);

        Destroy(minimapLines[index]);
        minimapLines.RemoveAt(index);

        updateModelBag();
    }

    #endregion


    #region Animation
    public void Die()
    {
        rb.velocity = new Vector3();
        rb.isKinematic = true;
        animator.SetTrigger("IsDead");
        _collider.isTrigger = true;
        orderTypes.RemoveRange(0, orderTypes.Count);
        minimapLines.RemoveRange(0, minimapLines.Count);
        effect_audio.PlayOneShot(DieSound);
    }

    public void Respawn(GameObject respawnPoint)
    {
        rb.isKinematic = false;
        isWalking = false;
        isRiding = false;
        isWearing = false;
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsRidingBike", isRiding);
        animator.SetBool("IsWearingMask", isWearing);

        animator.ResetTrigger("Mask");
        animator.ResetTrigger("Pill");
        animator.ResetTrigger("Spray");
        animator.ResetTrigger("IsDead");

        animator.Rebind();
        animator.Update(0);

        transform.position = respawnPoint.transform.position;
        transform.rotation = respawnPoint.transform.rotation;

        ShowModel();

        _collider.isTrigger = false;
    }

    public void rideBike(bool isRidingBike)
    {
        if (isRiding != isRidingBike)
        {
            isRiding = isRidingBike;
            cam.fieldOfView = isRiding ? 70 : 60;
            animator.SetBool("IsRidingBike", isRidingBike);
            modelHandler.SetBikeActive(isRidingBike);
            movement_audio.clip = isRiding ? MotorStartSound : WalkingSound;
            movement_audio.Play();
            if (!isRiding) effect_audio.PlayOneShot(MotorEndSound);
        }
    }

    public void wearMask()
    {
        animator.SetTrigger("Mask");
        effect_audio.PlayOneShot(MaskSound);
        StartCoroutine(ApplicationModel.GM.GameInvoke(() => { modelHandler.SetMaskActive(true); }, 1f));
        StartCoroutine(ApplicationModel.GM.GameInvoke(() => { modelHandler.SetMaskActive(false); }, ApplicationModel.GM.maskDuration));
    }
    public void spray()
    {
        animator.SetTrigger("Spray");
        effect_audio.PlayOneShot(SpraySound);
        modelHandler.SetSprayActive(true);
        StartCoroutine(ApplicationModel.GM.GameInvoke(() => { modelHandler.SetSprayActive(false); }, 1.5f));
    }

    public void pill()
    {
        animator.SetTrigger("Pill");
        //effect_audio.PlayOneShot(PillSound);
        modelHandler.SetPillActive(true);
        StartCoroutine(ApplicationModel.GM.GameInvoke(() => { modelHandler.SetPillActive(false); }, 2f));
    }

    //public void takeOffMask()
    //{
    //    wearMask(false);
    //}
    #endregion

    #region modelHandler
    public void SetColor(Color color)
    {
        if (modelHandler == null)
        {
            model.GetComponent<ModelHandler>().SetColor(color);
        }
        else
        {
            modelHandler.SetColor(color);
        }
    }

    public void SetTexture(Texture? texture)
    {
        if (modelHandler == null)
        {
            model.GetComponent<ModelHandler>().SetTexture(texture);
        }
        else
        {
            modelHandler.SetTexture(texture);
        }
    }

    public void HideModel()
    {
        model.SetActive(false);
    }

    public void ShowModel()
    {
        model.SetActive(true);
    }
    #endregion
}
