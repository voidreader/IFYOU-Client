//
//  TCGBGamebase.h
//  TCGBGamebase
//
//  Created by NHN on 2016. 6. 24..
//  © NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#ifndef TCGBGamebase_h
#define TCGBGamebase_h

NS_ASSUME_NONNULL_BEGIN

@class TCGBError;
@class TCGBConfiguration;
@class TCGBAuthToken;
@class TCGBTemporaryWithdrawalInfo;
@class TCGBAuthProviderProfile;
@class TCGBBanInfo;
@class TCGBServerPushMessage;
@class TCGBObserverMessage;
@class TCGBForcingMappingTicket;
@class TCGBTransferAccountInfo;
@class TCGBTransferAccountRenewConfiguration;
@class TCGBGamebaseEventMessage;

typedef NSDictionary* LaunchingInfo;

typedef void(^InitializeCompletion)(LaunchingInfo _Nullable launchingData, TCGBError* _Nullable error);
typedef void(^LoginCompletion)(TCGBAuthToken* _Nullable authToken, TCGBError* _Nullable error);
typedef void(^LogoutCompletion)(TCGBError* _Nullable error);
typedef void(^WithdrawCompletion)(TCGBError* _Nullable error);
typedef void(^TemporaryWithdrawCompletion)(TCGBTemporaryWithdrawalInfo* _Nullable info, TCGBError* _Nullable error);
typedef void(^RemoveMappingCompletion)(TCGBError* _Nullable error);
typedef void(^GetLanguageInterpreterCompletion)(NSDictionary* _Nullable data, TCGBError* _Nullable error);
typedef void(^TransferAccountCompletion)(TCGBTransferAccountInfo* _Nullable transferAccount, TCGBError* _Nullable error);
typedef void(^GamebaseEventHandler)(TCGBGamebaseEventMessage * message);

///TIP!: If you want not to see a kind of comments, use short-cut (shift + control + command + ⬅). Or want to see them use short-cut again (shift + control + command + ➡)

/** The TCGBGamebase class is core of TCGB service.
 */
@interface TCGBGamebase : NSObject

/**----------------------------------------------
 * @name Initialization
 * ----------------------------------------------
 */

/** This method initialize TCGBGamebase with TCGBConfiguration instance. InitializeCompletion return launching datas when TCGBGamebase has initialized and has completed getting launching informations from server.
 
 You must call this method first, after the app has launched.
 
 ### Usage Example
    
    - (void)onButtonInitialize {
        [TCGBGamebase setDebugMode:YES];
        TCGBConfiguration* config = [TCGBConfiguration configurationWithAppID:@"your app(project) ID" appVersion:@"your app version"];
        [TCGBGamebase initializeWithConfiguration:config completion:^(LaunchingInfo launchingData, TCGBError *error) {
            if ([TCGBGamebase isSuccessWithError:error] == YES) {
                NSLog(@"TCGBGamebase initialization is succeeded");
                // Check status of you app.
                // If status of app is maintenance or terminated service or etc, you must blocking UI and you should make user cannot log in your service.
                // You can use [TCGBLaunching launchingStatus] method to check status of your app.
            }
            else {
                NSLog(@"TCGBGamebase initialization is failed with error:[%@]", [error description]);
            }
        }];
    }
 
 @param configuration A parameter that were set appId, appVersion, zoneType and etc.
 @param completion A completion that is called after initializing TCGBGamebase completed.
 @see initializeWithConfiguration:launchOptions:completion:
 */
+ (void)initializeWithConfiguration:(TCGBConfiguration *)configuration completion:(nullable InitializeCompletion)completion;

/** This method initialize TCGBGamebase with TCGBConfiguration instance. InitializeCompletion return launching datas when TCGBGamebase has initialized and has completed getting launching informations from server.
 
 You must call this method first, after the app has launched.
 
 @param configuration A parameter that were set appId, appVersion, zoneType and etc.
 @param launchOptions An extra launch options when application needs to be customized initialization.
 @param completion A completion that is called after initializing TCGBGamebase completed.
 @see initializeWithConfiguration:launchOptions:completion:
 */
+ (void)initializeWithConfiguration:(TCGBConfiguration *)configuration launchOptions:(nullable NSDictionary *)launchOptions completion:(nullable InitializeCompletion)completion;

/**----------------------------------------------
 * @name Environment Setting & Getting
 * ----------------------------------------------
 */

/** Method that setting TCGBGamebase to debug mode. Debug mode can print logs that has debug, info and verbose log level on console app.
 
 You should call this method before calling initializeWithConfiguration:launchOptions:completion: method.
 
 ### Usage Example
        
     - (void)onButtonInitialize {
        [TCGBGamebase setDebugMode:YES];
        TCGBConfiguration* config = [TCGBConfiguration configurationWithAppID:@"your app(project) ID" appVersion:@"your app version"];
        [TCGBGamebase initializeWithConfiguration:config completion:^(LaunchingInfo launchingData, TCGBError *error) {
            if ([TCGBGamebase isSuccessWithError:error] == YES) {
                NSLog(@"TCGBGamebase initialization is succeeded");
                // Check status of you app.
                // If status of app is maintenance or terminated service or etc, you must blocking UI and you should make user cannot log in your service.
                // You can use [TCGBLaunching launchingStatus] method to check status of your app.
            }
            else {
                NSLog(@"TCGBGamebase initialization is failed with error:[%@]", [error description]);
            }
        }];
     }
 
 @param debugMode Boolean value that whether use debug mode or not.
 @see isDebugMode
 */
+ (void)setDebugMode:(BOOL)debugMode;


/** Method that got boolean value whether current SDK is running with debug mode or not.
 
 ### Usage Example
 
    -(void)onButtonIsDebugMode {
        BOOL isDebugMode = [TCGBGamebase isDebugMode];
        NSLog(@"Debug Mode is %@", isDebugMode ? @"on":@"off");
    }
 
 @return Boolean value that current SDK is running with debug mode or not.
 @see setDebugMode:
 */
+ (BOOL)isDebugMode;


/** Method that got SDK version of client
 
 ### Usage Example
 
    - (void)onButtonGetSDKVersion {
        NSString* version = [TCGBGamebase SDKVersion];
        NSLog(@"Client SDK Version = %@", version];
    }
 
 @return String value that got client sdk version.
 */
+ (NSString *)SDKVersion;


/** Method that got 'Projet ID (app ID)' is set by initialize TCGBGamebase.
 
 'Project ID (app ID)' is indicated on Toast Cloud Console Project List Page.
 
 ### Usage Example
 
    - (void)onButtonGeTCGBPID {
        NSString* appID = [TCGBGamebase appID];
        NSLog(@"App ID = %@", appID);
    }
 
 @return String value that got app id (project id).
 */
+ (nullable NSString *)appID;


/** Method that got an UserID that identify each user in Platform.

 You can get this after login has succeeded.
 
 ### Usage Example
 
    - (void)onButtonGetUserID {
        NSString* userID = [TCGBGamebase userID];
        NSLog(@"User ID = %@", userID];
    }
 
 @return String value that got userID.
 */
+ (nullable NSString *)userID;


/** Method that got an access token is taken from TCGB Platform server.
 
 You can get htis after login has succeeded.
 This value will be removed by logout, withdrawal and will be expired 15 day after last logged in.
 
 ### Usage Example
 
    - (void)onButtonAccessToken {
        NSString* TCGBAccessToken = [TCGBGamebase accessToken];
        NSLog(@"User ID = %@", TCGBAccessToken];
    }
 
 @return String value that got TCGB Access Token.
 */
+ (nullable NSString *)accessToken;


/** Return whether this project is sandbox mode or not.
 
 @return Boolean value whether this project is sandbox mode.
 */
+ (BOOL)isSandbox;


/** Method that distinguished the error object is whether succeeded or failed.
 
 ### Usage Example
 
    - (void)onButtonSuccessOfFail {
        [TCGBGamebase logoutWithCompletion:^(TCGBError *error) {
            BOOL isSuccess = [TCGBGamebase isSuccessWithError:error];
            if (isSuccess) {
                NSLog(@"logout is Success");
            }
            else {
                NSLog(@"logout is Fail");
            }
        }
    }
 
 @param error Object for TCGBError class what distinguished is whether succeeded or not.
 @return Boolean value that the TCGBError object is whether succeeded of failed.
 */
+ (BOOL)isSuccessWithError:(nullable TCGBError *)error;


/**----------------------------------------------
 * @name System Language & Country
 * ----------------------------------------------
 */

/**
 Get Language Code from device settings.
 
 @return language code from settings.
 @since Added 1.14.0.
 */
+ (NSString *)deviceLanguageCode;

/**
 Get carrier code from USIM.
 */
+ (nullable NSString *)carrierCode;

/**
 Get carrier name from USIM.
 */
+ (nullable NSString *)carrierName;

/**
 Get country code from USIM or device locale.
 */
+ (NSString *)countryCode;

/**
 Get country code from USIM.
 */
+ (NSString *)countryCodeOfUSIM;

/**
 Get country code from device locale.
 */
+ (NSString *)countryCodeOfDevice;

#pragma mark - TCGB Localization
/**----------------------------------------------
 * @name Display Language
 * ----------------------------------------------
 */

/**
 Set Display Language Code when initializing Gamebase SDK.
 
 @param languageCode    It represent language code (ISO-639)
 */
+ (void)setDisplayLanguageCode:( NSString *)languageCode;

/**
 Method that returns displayLanguage that you have set.
 
 @return String value of language code (ISO-639)
 */
+ (nullable NSString *)displayLanguageCode;





#pragma mark - TCGB Authentication & Authorization
/**----------------------------------------------
 * @name Login API
 * ----------------------------------------------
 */

/** Try to login with type of ID Provider. If after succeeded, completion returns TCGB Auth informations include authToken, userID.
 
 TCGBGamebase has loginForLastLoggedInProviderWithCompletion method known as Auto-Login. So before you try to login, you should try to login with loginForLastLoggedInProviderWithCompletion: method.
 If it is failed, you can try to login with loginWithType:viewController:completion: method
 
 ### Usage Example
 
    - (void)onButtonLogin {
        NSString* idpType = @"facebook";
        UIViewController* topViewController = nil;
 
        [TCGBGamebase loginForLastLoggedInProviderWithCompletion:^(TCGBAuthToken *authToken, TCGBError *error){
            if ([TCGBGamebase isSuccessWithError:error] == YES) {
                NSLog(@"Login is succeeded.");
            }
            else {
                if (error.code == TCGB_ERROR_SOCKET_ERROR || error.code == TCGB_ERROR_RESPONSE_TIMEOUT) {
                    NSLog(@"Retry loginForLastLoggedInProviderWithCompletion: or Notify to user -\n\terror[%@]", [error description]);
                }
                else {
                    NSLog(@"Try to login with loginWithType:viewController:completion:");
                    [TCGBGamebase loginWithType:idpType viewController:topViewController completion:^(TCGBAuthToken *authToken, TCGBError *error) {
                        if ([TCGBGamebase isSuccessWithError:error] == YES) {
                            NSLog(@"Login is succeeded.");
                        }
                        else {
                            NSLog(@"Login is failed.");
                        }
                    }];
                }
            }
        }];
    }
 
 @param type            String value of IDPType name (guest, facebook, iosgamecenter).
 @param viewController  UIViewController object that present login view controller.
 @param completion      callback that returned whether login success or fail and return TCGBAuthToken.
 */
+ (void)loginWithType:(NSString *)type viewController:(UIViewController *)viewController completion:(LoginCompletion)completion;
+ (void)loginWithType:(NSString *)type additionalInfo:(nullable NSDictionary *)additionalInfo viewController:(UIViewController *)viewController completion:(LoginCompletion)completion;

/** Try to login with credential information received from External SDK or OAuth.
 
 ### Usage Example
 
    - (void)onButtonLogin {
        UIViewController* topViewController = nil;
        NSString* facebookAccessToken = @"feijla;feij;fdklvda;hfihsdfeuipivaipef/131fcusp";
        NSMutableDictionary* credentialInfo = [NSMutableDictionary dictionary];
        credentialInfo[@"provider_name"] = @"facebook";
        credentialInfo[@"access_token"] = facebookAccessToken;
        [TCGBGamebase loginWithCredential:credentialInfo viewController:topViewController completion:^(TCGBAuthToken *authToken, TCGBError *error) {
            if ([TCGBGamebase isSuccessWithError:error] == YES) {
                NSLog(@"Login is succeeded.");
            }
            else {
                if (error.code == TCGB_ERROR_SOCKET_ERROR || error.code == TCGB_ERROR_RESPONSE_TIMEOUT) {
                    NSLog(@"Retry loginForLastLoggedInProviderWithCompletion: or Notify to user -\n\terror[%@]", [error description]);
                }
                else {
                    NSLog(@"Try to login with loginWithType:viewController:completion:");
                    [TCGBGamebase loginWithType:idpType viewController:topViewController completion:^(TCGBAuthToken *authToken, TCGBError *error) {
                        if ([TCGBGamebase isSuccessWithError:error] == YES) {
                            NSLog(@"Login is succeeded.");
                        }
                        else {
                            NSLog(@"Login is failed.");
                        }
                    }];
                }
            }
        }];
 
 @param credentialInfo NSDictionary value that contains provider_name (guest, facebook, iosgamecenter), access_token of IDP, access_token_secret of IDP that receive from IDP SDK.
 @param viewcontroller UIViewController object that present login view controller.
 @param completion     callback that returned whether login is success or fail and return TCGBAuthToken.
 */
+ (void)loginWithCredential:(NSDictionary *)credentialInfo viewController:(UIViewController *)viewcontroller completion:(LoginCompletion)completion;


/** Try to login for last logged in ID Provider. This method use local stored name of last logged in IDP.
 
 This method try to login with local stored TCGBAuthToken.
 
 ### Usage Example
    
    - (void)onButtonLogin {
        NSString* idpType = @"facebook";
        UIViewController* topViewController;
 
        [TCGBGamebase loginForLastLoggedInProviderWithViewController:topViewController completion:^(TCGBAuthToken *authToken, TCGBError *error){
            if ([TCGBGamebase isSuccessWithError:error] == YES) {
                NSLog(@"Login is succeeded.");
            }
            else {
                if (error.code == TCGB_ERROR_SOCKET_ERROR || error.code == TCGB_ERROR_RESPONSE_TIMEOUT) {
                    NSLog(@"Retry loginForLastLoggedInProviderWithCompletion: or Notify to user -\n\terror[%@]", [error description]);
                }
                else {
                    NSLog(@"Try to login with loginWithType:viewController:completion:");
                    [TCGBGamebase loginWithType:idpType viewController:topViewController completion:^(TCGBAuthToken *authToken, TCGBError *error) {
                        if ([TCGBGamebase isSuccessWithError:error] == YES) {
                            NSLog(@"Login is succeeded.");
                        }
                        else {
                            NSLog(@"Login is failed.");
                        }
                    }];
                }
            }
        }];
    }
 
 @param completion callback that return whether login success or fail and return TCGBAuthToken
 */
+ (void)loginForLastLoggedInProviderWithViewController:(UIViewController *)viewController completion:(LoginCompletion)completion ;


/**----------------------------------------------
 * @name Mapping API
 * ----------------------------------------------
 */

/** If you have logged in with an IDP Type. You can map with the other ID Providers, excluding Logged in IDP type.
 
 Mapping IDPs are just mapped to TCGB UserID. ID Provider type what mapped to UserID only can be removed from mapped UserID.
 @warning *Warning:* Logged in ID Provider type cannot be removed from UserID.
 @warning *Warning:* You can call addMapping after logged in.
 
 ### Usage Example
 
    - (void)onButtonAddMapping {
        NSString* idpType = @"facebook";
        
        [TCGBGamebase addMappingWithType:idpType viewController:nil completion:^(TCGBAuthToken *authToken, TCGBError *error) {
            if ([TCGBGamebase isSuccessWithError:error] == YES) {
                NSLog(@"addMapping is succeeded.");
            }
            else {
                NSLog(@"addMapping is failed with Error:[%@]", [error description]);
                if (error.code == TCGB_ERROR_AUTH_ADD_MAPPING_ALREADY_MAPPED_TO_OTHER_MEMBER || error.code == TCGB_ERROR_AUTH_ADD_MAPPING_ALREADY_HAS_SAME_IDP) {
                    NSLog(@"This ID Provider type has mapped to the other UserID");
                }
                else {
                    NSLog(@"addMapping is failed with error:[%@]", [error description]);
                }
            }
        }];
    }
 
 @param type            String value of IDPType name (guest, facebook, iosgamecenter).
 @param viewController  UIViewController object that present login view controller.
 @param completion      callback that return whether addMapping is success or fail and return TCGBAuthToken. If you received TCGB_ERROR_AUTH_ADD_MAPPING_ALREADY_MAPPED_TO_OTHER_MEMBER or TCGB_ERROR_AUTH_ADD_MAPPING_ALREADY_HAS_SAME_IDP error, A TCGBAuthToken received from completion is not for current userID, but for already mapped account.

 
 @see addMappingWithType:additionalInfo:viewController:completion:
 */
+ (void)addMappingWithType:(NSString *)type viewController:(UIViewController *)viewController completion:(LoginCompletion)completion;
+ (void)addMappingWithType:(NSString *)type additionalInfo:(nullable NSDictionary *)additionalInfo viewController:(UIViewController *)viewController completion:(LoginCompletion)completion;
+ (void)addMappingForciblyWithType:(NSString *)type forcingMappingKey:(NSString *)forcingMappingKey viewController:(UIViewController *)viewController completion:(LoginCompletion)completion;
+ (void)addMappingForciblyWithType:(NSString *)type forcingMappingKey:(NSString *)forcingMappingKey additionalInfo:(nullable NSDictionary *)additionalInfo viewController:(UIViewController *)viewController completion:(LoginCompletion)completion;

/** Try to addMapping with credential information received from External SDK or OAuth.
 
 ### Usage Example
 
     - (void)onButtonAddMapping {
         UIViewController* topViewController = nil;
 
         NSString* facebookAccessToken = @"feijla;feij;fdklvda;hfihsdfeuipivaipef/131fcusp"; // Get access token of your provider using provider SDK.
         NSMutableDictionary* credentialInfo = [NSMutableDictionary dictionary];
         credentialInfo[kTCGBAuthLoginWithCredentialProviderNameKeyname] = @"facebook";
         credentialInfo[kTCGBAuthLoginWithCredentialAccessTokenKeyname] = facebookAccessToken;
 
         [TCGBGamebase addMappingWithCredential:credentialInfo viewController:topViewController completion:^(TCGBAuthToken *authToken, TCGBError *error) {
             if ([TCGBGamebase isSuccessWithError:error] == YES) {
                 NSLog(@"AddMapping is succeeded.");
             }
             else if (error.code == TCGB_ERROR_SOCKET_ERROR || error.code == TCGB_ERROR_RESPONSE_TIMEOUT) {
                 NSLog(@"Retry addMapping")
             }
             else if (error.code == TCGB_ERROR_AUTH_ADD_MAPPING_ALREADY_MAPPED_TO_OTHER_MEMBER) {
                 NSLog(@"Already mepped to other member");
             }
             else {
                 NSLog(@"AddMapping Error - %@", [error description]);
             }
         }];
     }
 
 @param credentialInfo NSDictionary value that contains provider_name (guest, facebook, iosgamecenter), access_token of IDP, access_token_secret of IDP that receive from IDP SDK.
 @param viewcontroller UIViewController object that present login view controller.
 @param completion     callback that returned whether addMapping is success or fail and return TCGBAuthToken.
 */
+ (void)addMappingWithCredential:(NSDictionary *)credentialInfo viewController:(UIViewController *)viewcontroller completion:(LoginCompletion)completion;
+ (void)addMappingWithCredential:(NSDictionary *)credentialInfo forcingMappingKey:(NSString *)forcingMappingKey viewController:(UIViewController *)viewcontroller completion:(LoginCompletion)completion;


/** Disconnect ID Provider from UserID.
 
 @warning *Warning:* You cannot remove mapping last logged in ID Provider.
 
 ### Usage Example
    
    - (void)onButtonRemoveMapping {
        NSString* idpType = @"facebook";
        
    [TCGBGamebase removeMappingWithType:idpType viewController:topViewController completion:^(TCGBError *error) {
            NSLog(@"removeMappingWityType:%@ is %@", idpType, [TCGBGamebase isSuccessWithError:error] == YES ? @"success":@"fail");
        }];
    }
 
 
 @param type        String value of IDPType name (guest, facebook, iosgamecenter).
 @param completion  callback that return whether removeMapping is success or fail.
 */
+ (void)removeMappingWithType:(NSString *)type viewController:(nullable UIViewController *)viewController completion:(nullable RemoveMappingCompletion)completion;

/**----------------------------------------------
 * @name Logout API
 * ----------------------------------------------
 */

/** This method make user logout from TCGB Game Platform.
 
 This method try to logout from ID Provider SDK.
 
 ### Usage Example
 
    - (void)onButtonWithdrawal {
        [TCGBGamebase logoutWithViewController:topViewController completion:^(TCGBError *error) {
            NSLog(@"You have withdrew from TCGBGamebase : %@, [error debugDescription]);
        }];
    }
 
 @param completion callback that return whether logout is success or fail
 */
+ (void)logoutWithViewController:(nullable UIViewController *)viewController completion:(nullable LogoutCompletion)completion;

/**----------------------------------------------
 * @name Withdrawal API
 * ----------------------------------------------
 */

/** This method make user withdraw from TCGB Game Platform, but IdPs need to be unlink in each IdP user preference page.
 
 This method cannot drop out of IdP.
 
 ### Usage Example
 
     - (void)onButtonWithdrawal {
        [TCGBGamebase withdrawWithViewController:topViewController completion:^(TCGBError *error) {
            NSLog(@"You have withdrew from TCGBGamebase : %@, [error debugDescription]);
         }];
     }

 @param completion callback that return whether withdrawal is success or fail
 */
+ (void)withdrawWithViewController:(nullable UIViewController *)viewController completion:(nullable WithdrawCompletion)completion;


/**----------------------------------------------
 * @name TemporaryWithdrawal
 * ----------------------------------------------
 */

/** This method make user withdraw from TCGB Game Platform temporarily, but IdPs need to be unlink in each IdP user preference page.
 
 @param viewController UIViewController object that present view controller.
 @param completion     callback that returned whether temporary withdraw is success or fail and return TCGBTemporaryWithdrawalInfo.
 */
+ (void)requestTemporaryWithdrawalWithViewController:(nullable UIViewController *)viewController completion:(nullable TemporaryWithdrawCompletion)completion;

/** This method make user withdraw cancel from TCGB Game Platform temporarily.

@param viewController UIViewController object that present view controller.
@param completion     callback that returned whether cancel temporary withdraw is success or fail.
*/
+ (void)cancelTemporaryWithdrawalWithViewController:(nullable UIViewController *)viewController completion:(nullable WithdrawCompletion)completion;

/** This method make user withdraw from TCGB Game Platform immediately, but IdPs need to be unlink in each IdP user preference page.

@param viewController UIViewController object that present view controller.
@param completion     callback that returned whether immediately withdraw is success or fail.
*/
+ (void)withdrawImmediatelyWithViewController:(nullable UIViewController *)viewController completion:(nullable WithdrawCompletion)completion;

/**----------------------------------------------
 * @name Login Information
 * ----------------------------------------------
 */

/**
 Method that return which IDP have been logged in at last time.
 
 ### Usage Example
 
    - (void)onButtonGettingLastLoggedinProvider {
        NSString* lastProvider = [TCGBGamebase lastLoggedInProvider];
        NSLog(@"lastLoggedInProvider returns %@", lastProvider];
    }
 
 @return An String that has name of IDPs.
 */
+ (nullable NSString *)lastLoggedInProvider;


/**
 Method that returns which IDPs have been mapped to current UserID.
 
 ### Usage Example
    
    - (void)onButtonGettingMappedAuthList {
        NSArray* authList = [TCGBGamebase authMappingList];
        NSLog(@"authMappingList returns %@", [authList description]);
    }
 
 @return An Array that has name of IDPs.
 */
+ (nullable NSArray<NSString *> *)authMappingList;


/**----------------------------------------------
 * @name ID Provider Information
 * ----------------------------------------------
 */

/**
 Method that return UserID of ID Provider such as 'facebook', 'iosgamecenter' and etc.
 
 ### Usage Example
    
    - (void)onButtonGettingIDPUserName {
        NSString* userID = [TCGBGamebase authProviderUserIDWithIDPCode:@"facebook"];
        NSLog(@"authProvider UserID = %@", userID);
    }
 
 @param IDPCode     IDPCode string value that you want to get.
 @return A String value of user ID.
 */
+ (nullable NSString *)authProviderUserIDWithIDPCode:(NSString *)IDPCode;

/**
 Method that return AccessToken of ID Provider such as 'facebook', 'iosgamecenter' and etc.
 
 ### Usage Example
 
    - (void)onButtonGettingIDPAccessToken {
        NSString* accessToken = [TCGBGamebase authProviderAccessTokenWithIDPCode:@"facebook"];
        NSLog(@"authProvider AccessToken = %@", accessToken);
    }
 
 @param IDPCode     IDPCode string value that you want to get.
 @return A String value of AccessToken.
 */
+ (nullable NSString *)authProviderAccessTokenWithIDPCode:(NSString *)IDPCode;

/**
 Method that return Profile Information of ID Provider such as 'facebook', 'iosgamecenter' and etc.
 
 ### Usage Example
 
 - (void)onButtonGettingIDPProfile {
    NSString* profile = [TCGBGamebase authProviderProfileWithIDPCode:@"facebook"];
    NSLog(@"authProvider profile = %@", profile);
 }
 
 @param IDPCode     IDPCode string value that you want to get.
 @return A String value of profile.
 */
+ (nullable TCGBAuthProviderProfile *)authProviderProfileWithIDPCode:(NSString *)IDPCode;


/**----------------------------------------------
 * @name Ban Information
 * ----------------------------------------------
 */
#pragma mark - Ban Information
/**
 Method that return ban information of login response.
 
 ### Usage Example
 
 - (void)onButtonGettingIDPProfile {
    TCGBBanInfo* banInfo = [TCGBGamebase banInfo];
    NSLog(@"BanInfo = %@", [banInfo debugDescription]);
 }
 
 @return A ban information of login respons.
 */
+ (nullable TCGBBanInfo *)banInfo;

/**----------------------------------------------
 * @name Server Push Message Notification
 * ----------------------------------------------
 */
#pragma mark - Server Push Message Notification
/**
 Method that add an event handler to receive server push from Gamebase Server.
 
 ### Usage Example
 
 - (void)addServerPushEventHandler {
     void(^pushHandler)(TCGBServerPushMessage *) = ^(TCGBServerPushMessage *message) {
     NSString* msg = [NSString stringWithFormat:@"[Sample] receive server push =>\ntype: %@\ndata: %@", message.type, message.data];
     [self printLogAndShowAlertWithData:msg error:nil alertTitle:@"server push"];
     };
     [TCGBGamebase addServerPushEvent:pushHandler];
 }
 
 @since Added 1.8.0.
 */
+ (void)addServerPushEvent:(void(^)(TCGBServerPushMessage *))handler;

/**
 Method that remove an event handler.
 
 ### Usage Example
 
 - (void)removeServerPushEventHandler {
     void(^pushHandler)(TCGBServerPushMessage *) = ^(TCGBServerPushMessage *message) {
     NSString* msg = [NSString stringWithFormat:@"[Sample] receive server push =>\ntype: %@\ndata: %@", message.type, message.data];
     [self printLogAndShowAlertWithData:msg error:nil alertTitle:@"server push"];
     };
     [TCGBGamebase removeServerPushEvent:pushHandler];
 }
 
 @since Added 1.8.0.
 */
+ (void)removeServerPushEvent:(void(^)(TCGBServerPushMessage *))handler;

/**
 Method that remove all handlers.
 
 ### Usage Example
 
 - (void)removeAllEventHandlers {
    [TCGBGamebase removeAllServerPushEvent];
 }
 
 @since Added 1.8.0.
 */
+ (void)removeAllServerPushEvent;

/**----------------------------------------------
 * @name Long Pollilng Message Notification
 * ----------------------------------------------
 */
#pragma mark - Long Polling Message Notification
/**
 Method that add an observer handler to notify status changed by LaunchingStatus, Network Monitoring, User Info (ban).
 You can see the constants of message type in TCGBConstants.h (kTCGBObserverMessageTypeNetwork, kTCGBObserverMessageTypeLaunching, kTCGBObserverMessageTypeHeartbeat).
 
 ### Usage Example
 
 - (void)addObserverHandler {
     void(^observerHandler)(TCGBObserverMessage *) = ^(TCGBObserverMessage *message) {
     NSString* msg = [NSString stringWithFormat:@"[Sample] receive from observer =>\ntype: %@\ndata: %@", message.type, message.data];
     [self printLogAndShowAlertWithData:msg error:nil alertTitle:@"Observer"];
     };
 
    [TCGBGamebase addObserver:observerHandler];
 }
 
 @since Added 1.8.0.
 */
+ (void)addObserver:(void(^)(TCGBObserverMessage *))handler;

/**
 Method that remove an observer handler.
 
 ### Usage Example
 
 - (void)removeObserverHandler {
     void(^observerHandler)(TCGBObserverMessage *) = ^(TCGBObserverMessage *message) {
     NSString* msg = [NSString stringWithFormat:@"[Sample] receive from observer =>\ntype: %@\ndata: %@", message.type, message.data];
     [self printLogAndShowAlertWithData:msg error:nil alertTitle:@"Observer"];
     };
 
     [TCGBGamebase removeObserver:observerHandler];
 }
 
 @since Added 1.8.0.
 */
+ (void)removeObserver:(void(^)(TCGBObserverMessage *))handler;

/**
 Method that remove all observer handlers.
 
 ### Usage Example
 
 - (void)removeAllObserverHandlers {
    [TCGBGamebase removeAllObserver];
 }
 
 @since Added 1.8.0.
 */
+ (void)removeAllObserver;


/**----------------------------------------------
 * @name GamebaseEventHandler
 * ----------------------------------------------
 */
#pragma mark - EventHandler
/**
 Use this method to add handler for receiving event from ServerPush, Observer, RemoteNotification (foreground), AppStore Promoting IAP Purchase Result.
 
 ### Usage Example
 

 - (void)eventHandler_addEventHandler {
     void(^eventHandler)(TCGBGamebaseEventMessage *) = ^(TCGBGamebaseEventMessage * _Nonnull message) {
         NSLog(@"Received Message => %@", message);
     };
     
     static NSInteger count = 1;
     _gamebaseEventHandlers[@(count++)] = eventHandler;
     
     [TCGBGamebase addEventHandler:eventHandler];
 }
 
 @param handler Handler to receive Event
 @since Added 2.10.0
 */
+ (void)addEventHandler:(GamebaseEventHandler)handler;


/**
Use this method to remove handler for receiving event from ServerPush, Observer, RemoteNotification (foreground), AppStore Promoting IAP Purchase Result.

### Usage Example


- (void)eventHandler_removeEventHandler {
    [TCGBGamebase removeEventHandler:self->_gamebaseEventHandlers[n]];
    [self->_gamebaseEventHandlers removeObjectForKey:n];
}

@param handler Handler to remove
@since Added 2.10.0
*/
+ (void)removeEventHandler:(GamebaseEventHandler)handler;


/**
Use this method to remove all handlers for receiving event from ServerPush, Observer, RemoteNotification (foreground), AppStore Promoting IAP Purchase Result.

### Usage Example

- (void)eventHandler_removeAllEventHandler {
    [TCGBGamebase removeAllEventHandler];
}

@since Added 2.10.0
*/
+ (void)removeAllEventHandler;


/**----------------------------------------------
 * @name TransferAccount
 * ----------------------------------------------
 */
#pragma mark - TransferAccount
/**
 Use this method to retrieve already published Transfer Account information.
 
 ### Usage Example
 
 - (void)queryTransferAccount {
     [TCGBGamebase queryTransferAccountWithCompletion:^(TCGBTransferAccountInfo* transferAccount, TCGBError *error) {
        NSLog(@"Published TransferAccount => %@, error => %@", [transferAccount description], [error description]);
     }];
 }
 
 @param completion Handler to receive TransferAccount inforamtion.
 @since Added 2.1.0.
 
 */
+ (void)queryTransferAccountWithCompletion:(nullable TransferAccountCompletion)completion;

/**
 If you want to transfer an account that is guest to other device. You can use this method to publish id and password that called Transfer Account.
 To publish Transfer Account, an account must be logged in by Guest.
 After transfering guest account to other device, guest account on this device will be logged out and will not be able to authenticate with same guest account.
 
 ### Usage Example
 
 - (void)issueTransferAccount {
     [TCGBGamebase issueTransferAccountWithCompletion:^(TCGBTransferAccountInfo* transferAccount, TCGBError *error) {
        NSLog(@"Issued TransferAccount => %@, error => %@", [transferAccount description], [error description]);
     }];
 }
 
 @param completion Handler to receive TransferAccount inforamtion and an error.
 @since Added 2.1.0.
 */
+ (void)issueTransferAccountWithCompletion:(nullable TransferAccountCompletion)completion;

/**
 Use this method to change Transfer Account id or password.
 
 @param config Configuration for Transfer Account renewal.
 @param completion Handler to receive TransferAccount inforamtion and an error.
 @since Added 2.1.0
 */
+ (void)renewTransferAccountWithConfiguration:(TCGBTransferAccountRenewConfiguration *)config
                                   completion:(nullable TransferAccountCompletion)completion;

/**
 This method transfer the guest account to other device.
 If result is success, the guest account can be logged in to new device and logged out from old device.
 
 ### Usage Example
 
 - (void)transferOtherDevice {
    [TCGBGamebase transferAccountWithIdPLoginWithAccountId:@"1Aie0198" accountPassword:@"1Aie0199" completion:^(TCGBAuthToken* authToken, TCGBError* error) {
        NSLog(@"Transfered => %@",\nerror => %@", [authToken description], [error description]);
    }];
 }
 
 @param accountId TransferAccount id received from old device.
 @param accountPassword TransferAccount password from old device.
 @param completion Handler that include guest login informations and errors.
 @since Added 2.1.0.
 */
+ (void)transferAccountWithIdPLoginWithAccountId:(NSString *)accountId
                                 accountPassword:(NSString *)accountPassword
                                      completion:(nullable void(^)(TCGBAuthToken* _Nullable authToken, TCGBError* _Nullable error))completion;

/**----------------------------------------------
 * @name Application Life Cycle
 * ----------------------------------------------
 */
#pragma mark - Life Cycles (AppDelegate)
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
/**
 If you implement custom AuthAdapter with TCGBAuthAdapterDelegate to support custom IDPs and you need to be called 'application:willFinishLaunchingWithOptions:' in your adapter class.
 This method must be called in 'UIApplicationDelegate application:willFinishLaunchingWithOptions:' method.
 
 This method is *optional*.

 ### Usage Example

    - (BOOL)application:(UIApplication *)application willFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
        return [TCGBGamebase application:application willFinishLaunchingWithOptions:launchOptions];
    }
 
 @param application     Your singleton app object from UIApplication.
 @param launchOptions   A dictionary indicating the reason the app will launch.
 
 */
+ (BOOL)application:(UIApplication *)application willFinishLaunchingWithOptions:(NSDictionary *)launchOptions;


/**
 
 If you implement custom AuthAdapter with TCGBAuthAdapterDelegate to support custom IDPs and you need to be called 'application:didFinishLaunchingWithOptions:' in your adapter class.
 This method must be called in 'UIApplicationDelegate application:didFinishLaunchingWithOptions:' method.
 
 This method is *optional*.

 ### Usage Example
 
     - (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
        return [TCGBGamebase application:application didFinishLaunchingWithOptions:launchOptions];
     }
 
 @param application     Your singleton app object from UIApplication.
 @param launchOptions   A dictionary indicating the reason the app was launched.
 
 */
+ (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(nullable NSDictionary *)launchOptions;


+ (void)applicationDidBecomeActive:(UIApplication *)application;
// transitioning to background
+ (void)applicationDidEnterBackground:(UIApplication *)application;
// transitioning  to the inactive state
+ (void)applicationWillResignActive:(UIApplication *)application;
+ (void)applicationWillEnterForeground:(UIApplication *)application;
// termination
+ (void)applicationWillTerminate:(UIApplication *)application;

/// Responding to Notifications and Events
// When a remote notification arrives, the system calls the
+ (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult result))completionHandler;
// When a local notification fires, the system calls the
+ (void)application:(UIApplication *)application didReceiveLocalNotification:(UILocalNotification *)notification;
// When the user TCGBs a custom action in the alert for a remote or local notification’s, the system calls the
+ (void)application:(UIApplication *)application handleActionWithIdentifier:(nullable NSString *)identifier forRemoteNotification:(NSDictionary *)userInfo completionHandler:(void (^)(void))completionHandler;
+ (void)application:(UIApplication *)application handleActionWithIdentifier:(nullable NSString *)identifier forLocalNotification:(UILocalNotification *)notification completionHandler:(void (^)(void))completionHandler;
// For apps that want to initiate background downloads, the system calls the
+ (void)application:(UIApplication *)application performFetchWithCompletionHandler:(void (^)(UIBackgroundFetchResult result))completionHandler;
// For apps that use the NSURLSession class to perform background downloads, the system calls the
+ (void)application:(UIApplication *)application handleEventsForBackgroundURLSession:(NSString *)identifier completionHandler:(void (^)(void))completionHandler;
// When a low-memory condition occurs, the system notifies the app delegate by calling its
+ (void)applicationDidReceiveMemoryWarning:(UIApplication *)application;
// When a significant change in time occurs, the system notifies the app delegate by calling its
+ (void)applicationSignificantTimeChange:(UIApplication *)application;
// When the user locks the device, the system calls the app delegate’s
+ (void)applicationProtectedDataWillBecomeUnavailable:(UIApplication *)application;
// you can reestablish your references to the data in the app delegate’s
+ (void)applicationProtectedDataDidBecomeAvailable:(UIApplication *)application;


/**
 If you implement custom AuthAdapter with TCGBAuthAdapterDelegate to support custom IDPs and you need to be called 'application:openURL:options:' in your adapter class.
 This method must be called in 'UIApplicationDelegate application:openURL:options:' method.
 
 This method is *optional*.
 
 ### Usage Example
 
    - (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options {
        return [TCGBGamebase application:app openURL:url options:options];
    }
 
 @since iOS 9.0 <=
 
 @param app     Your singleton app object from UIApplication.
 @param url         The URL resource to open.
 @param options         A dictionary of URL handling options.
 
 @return YES if the delegate sucessfully handled the request or NO if the attempt to open the URL resource failed.
 */
+ (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options; // available iOS9 and later


/**
 If you implement custom AuthAdapter with TCGBAuthAdapterDelegate to support custom IDPs and you need to be called 'application:openURL:sourceApplication:annotation:' in your adapter class.
 This method must be called in 'UIApplicationDelegate application:openURL:sourceApplication:annotation:' method.
 
 This method is *optional*.
 
 ### Usage Example
 
    + (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation {
        return [TCGBGamebase application:application openURL:url sourceApplication:sourceApplication annotation:annotation];
    }
 
 @since iOS 4.2 ~ 8.x
 
 @param application     Your singleton app object from UIApplication.
 @param url             The URL resource to open.
 @param sourceApplication   The bundle ID of the app that is requesting your app to open the URL.
 @param annotation      A Property list supplied by the source app to communicate information to the receiving app.
 
 @return YES if the delegate sucessfully handled the request or NO if the attempt to open the URL resource failed.
 */
+ (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(nullable NSString *)sourceApplication annotation:(id)annotation; // available iOS 4.2 ~ iOS8(deprecated at 9.0)


/**
 If you implement custom AuthAdapter with TCGBAuthAdapterDelegate to support custom IDPs and you need to be called 'application:handleOpenURL:' in your adapter class.
 This method must be called in 'UIApplicationDelegate application:handleOpenURL:' method.
 
 This method is *optional*.
 
 ### Usage Example
 
    + (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url {
        return [TCGBGamebase application:application handleOpenURL:url];
    }
 
 @since iOS 4.2 ~ 8.x
 
 @param application     Your singleton app object from UIApplication.
 @param url             The URL resource to open.
 
 @return YES if the delegate sucessfully handled the request or NO if the attempt to open the URL resource failed.
 */
+ (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url; // available iOS2 ~ iOS8(deprecated at 9.0)


#pragma mark - Life Cycles (SceneDelegate)
/**
This method must be called in 'UIApplicationDelegate application:didDiscaredSceneSessions:' method.

This method is *optional*.

### Usage Example

   - (void)application:(UIApplication *)application didDiscardSceneSessions:(NSSet<UISceneSession *> *)sceneSessions {
       [TCGBGamebase application:application didDiscaredSceneSessions:sceneSessions];
   }


@param application     Your singleton app object from UIApplication.
@param sceneSessions The session objects associated with the discarded scens.
*/
+ (void)application:(UIApplication *)application didDiscardSceneSessions:(NSSet<UISceneSession *> *)sceneSessions API_AVAILABLE(ios(13.0));

/**
This method must be called in 'UISceneDelegate scene:willConnectToSession:options:' method.

This method is *optional*.

### Usage Example

   - (void)scene:(UIScene *)scene willConnectToSession:(UISceneSession *)session options:(UISceneConnectionOptions *)connectionOptions {
       [TCGBGamebase scene:scene willConnectToSession:session options:connectionOptions];
   }


@param scene The scene object being connected to your app.
@param session The session object containing details about the scene's configuration.
@param connectionOptions Additional options for configuring the scene.
*/
+ (void)scene:(UIScene *)scene willConnectToSession:(UISceneSession *)session options:(UISceneConnectionOptions *)connectionOptions API_AVAILABLE(ios(13.0));

/**
This method must be called in 'UISceneDelegate sceneDidDisconnect:' method.

This method is *optional*.

### Usage Example

   - (void)sceneDidDisconnect:(UIScene *)scene {
       [TCGBGamebase sceneDidDisconnect:scene];
   }


@param scene The scene that UIKit disconnected from your app.
*/
+ (void)sceneDidDisconnect:(UIScene *)scene API_AVAILABLE(ios(13.0));


/**
This method must be called in 'UISceneDelegate sceneDidBecomeActive:' method.

This method is *optional*.

### Usage Example

   - (void)sceneDidBecomeActive:(UIScene *)scene {
       [TCGBGamebase sceneDidBecomeActive:scene];
   }


@param scene The scene that became active and is now responding to user events.
*/
+ (void)sceneDidBecomeActive:(UIScene *)scene API_AVAILABLE(ios(13.0));

/**
This method must be called in 'UISceneDelegate sceneWillEnterForeground' method.

This method is *optional*.

### Usage Example

   - (void)sceneWillEnterForeground:(UIScene *)scene {
       [TCGBGamebase sceneDidBecomeActive:scene];
   }


@param scene The scene that became active and is now responding to user events.
*/
+ (void)sceneWillEnterForeground:(UIScene *)scene API_AVAILABLE(ios(13.0));

/**
This method must be called in 'UISceneDelegate sceneDidEnterBackground' method.

This method is *optional*.

### Usage Example

   - (void)sceneDidEnterBackground:(UIScene *)scene {
       [TCGBGamebase sceneDidEnterBackground:scene];
   }


@param scene The scene that entered the background.
*/
+ (void)sceneDidEnterBackground:(UIScene *)scene API_AVAILABLE(ios(13.0));

/**
This method must be called in 'UISceneDelegate scene:openURLContexts:' method.

This method is *optional*.

### Usage Example

   - (void)scene:(UIScene *)scene openURLContexts:(NSSet<UIOpenURLContext *> *)URLContexts {
       [TCGBGamebase scene:scene openURLContexts:URLContexts];
   }


@param scene The scene that UIKit asks to open the URL.
@param URLContexts Each object contains one URL to open and any additional information needed to open that URL.
*/
+ (void)scene:(UIScene *)scene openURLContexts:(NSSet<UIOpenURLContext *> *)URLContexts API_AVAILABLE(ios(13.0));

#pragma clang diagnostic pop
@end


@interface TCGBGamebase (deprecated)

/**
 @since Added 1.4.0.
 @deprecated As of release 1.14.0, use deviceLanguageCode method instead.
 */
+ (NSString *)languageCode DEPRECATED_MSG_ATTRIBUTE("Use deviceLanguageCode method instead.");

@end

NS_ASSUME_NONNULL_END

#endif
