//
//  TCGBAuthToken.h
//  Gamebase
//
//  Created by NHN on 2016. 12. 5..
//  © NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "TCGBGraceBanInfo.h"

NS_ASSUME_NONNULL_BEGIN

@class TCGBProviderAuthCredential;
@class TCGBProviderAuthToken;
@class TCGBProviderAuthProfile;
@class TCGBProviderAuthExtra;
@class TCGBTemporaryWithdrawalInfo;

/** The TCGBAuthMappingInfo class indicates already mapped provider list and several information toward logged-in account. This is dictinoary formmated.
 */
@interface TCGBAuthMappingInfo : NSDictionary
@end

/** The TCGBToken class have access token and access token secret.
 */
@interface TCGBToken : NSObject

/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/** 
 Access Token
 */
@property (nonatomic, strong) NSString* accessToken;

/** 
 Access Token Secret
 */
@property (nonatomic, strong, nullable) NSString* accessTokenSecret;

@end

/** The TCGBMember has user information who logged in.
 */
@interface TCGBMember : NSObject

/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/** 
 User ID
 */
@property (nonatomic, strong) NSString* userId;

/**
 Notify wheter this user is valid or not
 */
@property (nonatomic, strong) NSString* valid;

/**
 Application ID
 */
@property (nonatomic, strong) NSString* appId;

/**
 Registered Date
 */
@property (nonatomic, assign) long long      regDate;

/**
 Last Login Date
 */
@property (nonatomic, assign) long long      lastLoginDate;

/**
 Mapped provider list and information
 @see TCGBAuthMappingInfo
 */
@property (nonatomic, strong, nullable) NSArray<TCGBAuthMappingInfo *>*  authList;

/**
 Temporary withdrawal information
 */
@property (nonatomic, strong, nullable) TCGBTemporaryWithdrawalInfo* temporaryWithdrawal;

/**
 Grace ban information
 */
@property (nonatomic, strong, nullable) TCGBGraceBanInfo *graceBanInfo;

@end


/** The TCGBTemporaryWithdrawalInfo class has information about pending withdrawal.
 */
@interface TCGBTemporaryWithdrawalInfo : NSObject

/**
 grace period (epoch time in milliseconds)
 */
@property (nonatomic, assign) long gracePeriodDate;

+ (TCGBTemporaryWithdrawalInfo *)temporaryWithdrawalInfoWithDictionary:(NSDictionary *)temporaryWithdrawalDic;
- (NSDictionary *)JSONObject;
- (NSString *)JSONString;

@end



/** The TCGBAuthToken represents an authenticated token and user information for using TCGB services.
 */
@interface TCGBAuthToken : NSObject

/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/**
 TCGB Token information
 @see TCGBToken
 */
@property (nonatomic, strong) TCGBToken*     tcgbToken;

/**
 User information
 @see TCGBMember
 */
@property (nonatomic, strong) TCGBMember*    tcgbMember;

/**---------------------------------------------------------------------------------------
 * @name Initialization
 *  ---------------------------------------------------------------------------------------
 */

/**
 Creates a TCGBAuthToken instance.
 
 @param json Creates a TCGBAuthToken instance using a dictionary.
 */
+ (TCGBAuthToken *)tcgbAuthTokenWithJSONObject:(NSDictionary *)json;

/**
 Creates a TCGBAuthToken instance.
 
 @param responsed Creates a TCGBAuthToken instance using a dictionary.
 */
+ (TCGBAuthToken *)authTokenFromTCGBServerWithResponsed:(NSDictionary *)responsed;

/**---------------------------------------------------------------------------------------
 * @name Initialization
 *  ---------------------------------------------------------------------------------------
 */

/**
 @return Mapped ID Provider List
 */
- (nullable NSArray<NSString *> *)mappingAuthList;

/**
 @return stringify with this TCGBAuthToken into JSON string format.
 */
- (NSString *)JSONString;

@end

NS_ASSUME_NONNULL_END
