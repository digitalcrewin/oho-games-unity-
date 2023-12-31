﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConstants : MonoBehaviour
{
//    Poker Time Server 3 (Demo)

//IP: 3.137.10.123

    public static int TURN_TIME = 20;
    public static bool playerblind = false;
    public static bool showmatch = false;
    public static bool sideShowMatch = false;
    public static int timerStart = 0;
    public static int playerbetAmount = 0;
    public static string sideShowRequesterId = "";
    public static bool poker = true;
    public static float maxChaal = 0;


    


    #region ANIMATIONS
    public const float CARD_ANIMATION_DURATION = 0.27f;/*0.6f;*/
    public const float BET_PLACE_ANIMATION_DURATION = 0.5f;
    public const float LOCAL_BET_ANIMATION_DURATION = 1f;

    #endregion

    #region GAME_CONSTANTS
    public const int NUMBER_OF_CARDS_IN_DECK = 52;
    public static int[] NUMBER_OF_CARDS_PLAYER_GET_IN_MATCH = { 2, 4, 10 };
    public static int NUMBER_OF_CARDS_PLAYER_GET_IN_MATCH_IN_TEEN_PATTI = 3;
    public const int NUMBER_OF_COMMUNITY_CARDS = 5;
    public const float BUFFER_TIME = 3f;
    #endregion


    #region WEB
    public const float NETWORK_CHECK_DELAY = 2f;
    public const int API_RETRY_LIMIT = 5;
    public const int API_TIME_OUT_LIMIT = 50;

    //public const string BASE_URL = "http://15.206.57.137"; //Production
    public const string BASE_URL = "http://15.206.169.205"; //Dev

    public const string SOCKET_URL_FLASH = "http://3.17.201.78" + ":8888";

    public const string API_URL = BASE_URL + ":4000/api/v1"; //":3000";
    public const string SOCKET_URL = BASE_URL + ":3333";
    public const string CLUB_SOCKET_URL = BASE_URL + ":3334";
    public const string TOURNAMENT_SOCKET_URL = BASE_URL + ":3335";

    public const string GAME_PORTAL_URL = BASE_URL;// "http://192.168.0.151";//"http://3.17.201.78";//"http://3.137.10.123";//90// "http://3.17.201.78";//"http://18.191.15.121";//"http://3.6.137.204";



    public static string[] GAME_URLS =
    {
        API_URL +"/auth/register",
        API_URL +"/auth/verify-otp",
        API_URL +"/auth/resend-otp",
        API_URL +"/auth/reset-password",
        API_URL +"/auth/login",
        API_URL +"/auth/forgot-password",
        API_URL +"/auth/verify-otp-for-forgot-password",
        API_URL +"/user/profile",
		API_URL + "/user/update-profile",
        API_URL + "/user/add-amount",
		API_URL +"/createClub",
        API_URL +"/joinClubRequest",
        API_URL +"/getClubListByUserId",
        API_URL +"/getClubMemberListByClubId",
        API_URL +"/changePlayerRoleAndStatus",
        API_URL +"/deleteUserClubJoinedRedquest",
        API_URL +"/getShops",
        API_URL +"/getCardFeatures",
        API_URL +"/getRooms",
        API_URL +"/updateUserDetails",
        API_URL +"/updateFirebaseToken",
        API_URL +"/getFirebaseNotifiction",
        API_URL +"/updateFirebaseNotifiction",
        API_URL +"/getForum",
        API_URL +"/postLike",
        API_URL +"/getComment",
        API_URL +"/postComment",
        SOCKET_URL +"/getMissions",
        API_URL +"/getCountries",
        API_URL+"/getAvatars",
        API_URL+"/updateUserSettings",
        SOCKET_URL +"/getTopPlayers",
        API_URL+"/updateTableSettings",
        API_URL+"/getTableSettingData",
        API_URL+"/getUserDetails",
        API_URL+"/rewardCoins",
        API_URL+"/getFriendList",
        API_URL+"/getAllFriendRequest",
        API_URL+"/sendFriendRequest",
        API_URL+"/updateRequestStatus",
        API_URL +"/getSpinWheelItems",
        API_URL +"/setSpinWheelWinning",
        SOCKET_URL+"/shopItem",
        API_URL+"/getSpinWinnerList",
        API_URL+"/deductFromWallet",
        API_URL+"/getFrames",
        API_URL+"/updateClub",
        API_URL+"/getUnionClubList",
        API_URL+"/postNotification",
        API_URL+"/getNotifications",
        API_URL+"/readNotification",
        API_URL+"/getPendingClubJoinRequest",
        API_URL+"/rateClub",
        API_URL+"/user/email-verification/",
        API_URL+"/unlinkEmail",
        API_URL+"/changePassword",
        API_URL+"/redeemCoupon",
        SOCKET_URL+"/userLoginLogs",
        API_URL+"/createForum",
        API_URL+"/sendOut",
        API_URL+"/claimBack",
        API_URL+"/getTradeHistory",
        API_URL+"/updateSleepMode",
        API_URL+"/addTicket",
        API_URL+"/getTickets",
        API_URL+"/sendTicket",
        API_URL+"/convertTicket",
        API_URL+"/sendVIP",
        SOCKET_URL+"/redeemDailyMission",
        API_URL+"/updateProfile",
        API_URL+"/getClubDetails",
        API_URL+"/addPTchips",
        API_URL+"/addMultiAccountConnectRequest",
        API_URL+"/getMultiAccountPendingRequests",
        API_URL+"/updateMultiAccountRequestStatus",
        API_URL+"/getMyConnectedAccounts",
        API_URL+"/forgotPassword",
        API_URL+"/updateTemplateStatus",
        API_URL+"/createTemplate",
        API_URL+"/getTemplates",
        SOCKET_URL+"/getRealTimeData",
        API_URL+"/getTableHandHistory",
        API_URL+"/editMemberDetails",
        API_URL+"/getAgentDetails",
        API_URL+"/getDownlineList",
        API_URL+"/addDownliners",
        API_URL+"/getJackpotDetailByClubId",
        API_URL+"/topUpJackpot",
        API_URL+"/getTopUpDetailsByClubId",
        API_URL+"/onOffJackpot",
        API_URL+"/createJackpot",
        SOCKET_URL + "/getSeatObject",
        SOCKET_URL+"/getGameHistroy",
        API_URL+"/razorpay/payments/order", //"/beforPayment",
        API_URL+"/afterPayment",
        //lobbyTeenUrl+"/tp_getRooms",
        API_URL+"/getClubDataAnalytics",
        API_URL+"/getClubTableDataAnalytics",
        API_URL+"/getClubUserDetail",
        API_URL+"/verifyTablePassCode",
        API_URL+"/careerData",
        API_URL+"/user/redeem-amount",
        API_URL+"/withdrawals/all",
        API_URL+"/banks",
        API_URL+"/banks/fetch",
        API_URL+"/banks/primary",
        API_URL +"/newUserLogin",
        "http://65.1.233.18/api/Signup",
        "http://65.1.233.18/api/Signin",
        API_URL +"/checkUserName",
        API_URL + "/verify/mobile/sendotp",
        API_URL + "/verify/mobile",
        API_URL + "/verify/sendlink"
	};
    #endregion


    //--------------------------------------------------- TEEN PATTI ---------------------------------------------------
    #region WEB_TEEN_PATTI

    public const string BASE_URL_TP = "http://3.17.201.78";//"http://3.137.10.123";

    public const string API_URL_TP = BASE_URL_TP + ":3000";

    public static string[] GAME_URLS_TP =
    {
        API_URL_TP +"/tp_createClub",
        API_URL_TP +"/tp_joinClubRequest",
        API_URL_TP +"/tp_updateClub",
        API_URL_TP +"/tp_createTable",
        API_URL_TP +"/tp_changePlayerRoleAndStatus",
        API_URL_TP +"/tp_getPendingClubJoinRequest",
        API_URL_TP +"/tp_deleteUserClubJoinedRedquest",
        API_URL_TP +"/tp_getTemplates",
        API_URL_TP +"/tp_updateTemplateStatus",
        API_URL_TP +"/tp_getClubListByUserId",
        API_URL_TP +"/tp_getClubDetails",
        API_URL_TP +"/tp_getClubMemberListByClubId",
        API_URL_TP +"/tp_postNotification",
        API_URL_TP +"/tp_getNotifications",
        API_URL_TP +"/tp_addPTchips",
        API_URL_TP +"/tp_rateClub",
        API_URL_TP +"/tp_sendOut",
        API_URL_TP +"/tp_claimBack",
        API_URL_TP +"/tp_getTradeHistory",
        API_URL_TP +"/tp_sendSupportMessage",
        API_URL_TP +"/tp_getRooms",
        API_URL_TP +"/tp_readNotification",
        API_URL_TP +"/tp_updateAutoStraddle",
        API_URL_TP +"/tp_tagMember",
        API_URL_TP +"/tp_editMemberDetails",
        API_URL_TP +"/tp_getAgentDetails"
    };

    
    #endregion
}

[System.Serializable]
public enum RequestType
{
    Registration,
    VerifyOtp,
    ResendOtp,
    ResetPassword,
    Login,
    ForgotPassword,
    VerifyOtpForForgotPassword,
    Profile,
	EditProfile,
    AddCash,
	CreateClub,
    SendClubJoinRequest,
    GetClubList,
    GetClubMemberList,
    ChangePlayerRoleInClub,
    DeleteUserJoinRequest,
    GetShopValues,
    GetVIPPrivilege,
    GetLobbyRooms,
    UpdateUserBalance,
    SendNotificationToken,
    GetNotificationMessage,
    UpdateNotificationMessage,
    GetForum,
    PostLike,
    GetComment,
    PostComment,
    GetMissions,
    GetCountryList,
    GetAvatars,
    UpdateUserSettings,
    GetTopPlayers,
    UpdateTableSettings,
    GetTableSettingData,
    GetUserDetails,
    GetRewardCoins,
    GetFriendList,
    GetAllFriendRequest,
    SendFriendRequest,
    UpdateRequestStatus,
    GetSpinWheelItems,
    SetSpinWheelWinning,
    GetInGameShopValue,
    getSpinWinnerList,
    deductFromWallet,
    getFrames,
    UpdateClub,
    GetUnionClubList,
    PostNotification,
    GetNotification,
    ReadNotification,
    GetPendingClubJoinRequest,
    RateClub,
    emailVerified,
    unlinkEmail,
    changePassword,
    redeemCoupon,
    userLoginLogs,
    createForum,
    SendChipsOut,
    ClaimBackChips,
    GetTradeHistory,
    UpdateSleepMode,
    AddTicket,
    GetTickets,
    SendTickets,
    ConvertTicket,
    SendVIP,
    redeemDailyMission,
    updateProfile,
    GetClubDetails,
    AddPTChips,
    AddMultiAccountConnectRequest,
    GetMultiAccountPendingRequests,
    UpdateMultiAccountRequestStatus,
    GetMyConnectedAccounts,
    ForgotPasswordNew,
    UpdateTemplateStatus,
    CreateTemplate,
    GetTemplates,
    RealtimeResult,
    GetTableHandHistory,
    EditClubMemberDetails,
    GetAgentDetails,
    GetDownlineList,
    AddDownliners,
    GetJackpotDetailByClubId,
    TopUpJackpot,
    GetTopUpDetailsByClubId,
    OnOffJackpot,
    CreateJackpot,
    GetSeatObject,
    GetGameHistory,
    BeforePayment,
    AfterPayment,
    //LobbyRoomsTeenPatti,
    GetClubDataAnalytics,
    GetClubTableDataAnalytics,
    getClubUserDetail,
    verifyTablePassCode,
    CareerData,
    WithdrawRequest,
    TransHistory,
    Banks,
    FetchBank,
    SetPrimaryBank,
    GuestLogin,
    FantasySignUp,
    FantasyLogin,
    CheckUserName,
    MobileSendOtp,
    MobileVerify,
    SendRegVerificationEmail
}

[System.Serializable]
public enum RequestTypeTP
{
    CreateClub,
    SendClubJoinRequest,
    UpdateClub,
    CreateTable,
    ChangePlayerRoleAndStatus,
    GetPendingClubJoinRequest,
    DeleteUserClubJoinedRedquest,
    GetTemplates,
    UpdateTemplateStatus,
    GetClubList,
    GetClubDetails,
    GetClubMemberList,
    PostNotification,
    GetNotification,
    AddPTChips,
    RateClub,
    SendChipsOut,
    ClaimBackChips,
    GetTradeHistory,
    SendSupportMessage,
    GetRooms,
    ReadNotification,
    UpdateAutoStraddle,
    TagMember,
    EditClubMemberDetails,
    GetAgentDetails,
}