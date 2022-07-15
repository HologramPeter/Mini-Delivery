// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using System;
using System.Collections.Generic;

public class AssetContract
{
    public string address ;
    public string asset_contract_type ;
    public DateTime created_date ;
    public string name ;
    public object nft_version ;
    public string opensea_version ;
    public int owner ;
    public string schema_name ;
    public string symbol ;
    public object total_supply ;
    public string description ;
    public object external_link ;
    public object image_url ;
    public bool default_to_fiat ;
    public int dev_buyer_fee_basis_points ;
    public int dev_seller_fee_basis_points ;
    public bool only_proxied_transfers ;
    public int opensea_buyer_fee_basis_points ;
    public int opensea_seller_fee_basis_points ;
    public int buyer_fee_basis_points ;
    public int seller_fee_basis_points ;
    public object payout_address ;
}

public class DisplayData
{
    public string card_display_style ;
}

public class Collection
{
    public string banner_image_url ;
    public object chat_url ;
    public DateTime created_date ;
    public bool default_to_fiat ;
    public string description ;
    public string dev_buyer_fee_basis_points ;
    public string dev_seller_fee_basis_points ;
    public object discord_url ;
    public DisplayData display_data ;
    public object external_url ;
    public bool featured ;
    public string featured_image_url ;
    public bool hidden ;
    public string safelist_request_status ;
    public string image_url ;
    public bool is_subject_to_whitelist ;
    public string large_image_url ;
    public object medium_username ;
    public string name ;
    public bool only_proxied_transfers ;
    public string opensea_buyer_fee_basis_points ;
    public string opensea_seller_fee_basis_points ;
    public string payout_address ;
    public bool require_email ;
    public object short_description ;
    public string slug ;
    public object telegram_url ;
    public object twitter_username ;
    public string instagram_username ;
    public object wiki_url ;
}

public class User
{
    public string username ;
}

public class Owner
{
    public User user ;
    public string profile_img_url ;
    public string address ;
    public string config ;
}

public class Creator
{
    public User user ;
    public string profile_img_url ;
    public string address ;
    public string config ;
}

[Serializable]
public class Asset
{
    public int id ;
    public int num_sales ;
    public object background_color ;
    public string image_url ;
    public string image_preview_url ;
    public string image_thumbnail_url ;
    public object image_original_url ;
    public object animation_url ;
    public object animation_original_url ;
    public string name ;
    public string description ;
    public object external_link ;
    public AssetContract asset_contract ;
    public string permalink ;
    public Collection collection ;
    public object decimals ;
    public object token_metadata ;
    public Owner owner ;
    public object sell_orders ;
    public Creator creator ;
    public List<object> traits ;
    public object last_sale ;
    public object top_bid ;
    public object listing_date ;
    public bool is_presale ;
    public object transfer_fee_payment_token ;
    public object transfer_fee ;
    public string token_id ;
}

[Serializable]
public class OpenSea
{
    public string next ;
    public object previous ;
    public List<Asset> assets ;
}


