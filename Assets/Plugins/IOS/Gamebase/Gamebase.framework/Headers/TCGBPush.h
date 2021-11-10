//
//  TCGBPush.h
//  Gamebase
//
//  Created by NHN on 2016. 5. 31..
//  © NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "TCGBPushConfiguration.h"
#import "TCGBNotificationOptions.h"
#import "TCGBPushTokenInfo.h"
#import "TCGBPushDelegate.h"
#import "TCGBPushAgreement.h"
#import "TCGBError.h"

NS_ASSUME_NONNULL_BEGIN

@class TCGBError;

extern NSString* const kTCGBPushEnabledKeyname;              // keyname of pushEnabled property
extern NSString* const kTCGBPushADAgreementKeyname;          // keyname of ADAgreement property
extern NSString* const kTCGBPushADAgreementNightKeyname;     // keyname of ADAgreementNight property
extern NSString* const kTCGBPushForegroundEnabled;           // keyname of enableForeground property
extern NSString* const kTCGBPushBadgeEnabled;                // keyname of enableBadge property
extern NSString* const kTCGBPushSoundEnabled;                // keyname of enableSound property
extern NSString* const kTCGBPushDisplayLanguageCodeKeyname;  // keyname of displayLanguageCode property

/** The TCGBPush class provides registering push token API to ToastCloud Push Server and querying push token API.
 */
@interface TCGBPush : NSObject

/**
 Register push token to ToastCloud Push Server.
 
 @param configuration The configuration which has pushEnabled, ADAgreement and AdAgreementNight.
 @param completion callback
 
 @see TCGBPushConfiguration
 @since Added 1.4.0.
 */
+ (void)registerPushWithPushConfiguration:(TCGBPushConfiguration *)configuration completion:(nullable void(^)(TCGBError * _Nullable error))completion;

/**
 Register push token to ToastCloud Push Server.
 
 @param configuration The configuration which has pushEnabled, ADAgreement and AdAgreementNight.
 @param notificationOptions The notificationOptions which has foregroundEnabled, badgeEnabled, soundEnabled.
 @param completion callback
 
 @see TCGBPushConfiguration
 @see TCGBNotificationOptions
 @since Added 2.15.0.
 */
+ (void)registerPushWithPushConfiguration:(TCGBPushConfiguration *)configuration notificationOptions:(nullable TCGBNotificationOptions *)notificationOptions completion:(nullable void(^)(TCGBError * _Nullable error))completion;

/**
 Query push token to ToastCloud Push Server.
 
 @param completion callback, this callback has TCGBPushConfiguration information.
 
 @see TCGBPushConfiguration
 @since Added 1.4.0.
 */
+ (void)queryPushWithCompletion:(void(^)(TCGBPushConfiguration * _Nullable configuration, TCGBError * _Nullable error))completion;

/**
Query push token information to ToastCloud Push Server.

@param completion callback, this callback has TCGBPushTokenInfo information.

@see TCGBPushTokenInfo
@since Added 2.15.0.
*/
+ (void)queryTokenInfoWithCompletion:(void(^)(TCGBPushTokenInfo * _Nullable tokenInfo, TCGBError * _Nullable error))completion;

/**
 Set SandboxMode.
 
 @param isSandbox `YES` if application is on the sandbox mode.
 @since Added 1.4.0.
 */
+ (void)setSandboxMode:(BOOL)isSandbox;

/**
Get notificationOptions.

@since Added 2.15.0.
*/
+ (nullable TCGBNotificationOptions *)notificationOptions;

@end

NS_ASSUME_NONNULL_END
