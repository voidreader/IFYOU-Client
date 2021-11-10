//
//  TCGBGraceBanInfo.h
//  Gamebase
//
//  Created by NHN on 2021/09/01.
//  Copyright © 2021 NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "TCGBValueObject.h"

@class TCGBPaymentStatus;
@class TCGBReleaseRuleCondition;

NS_ASSUME_NONNULL_BEGIN

/** The TCGBGraceBanInfo class have information about grace ban.
 */
@interface TCGBGraceBanInfo : NSObject<TCGBValueObject>

/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/**
 Grace period expiration date (epoch time in milliseconds)
 If the payment conditions set before the period are satisfied, user's ban will be released.
 */
@property (nonatomic, assign) long long gracePeriodDate;

/**
 Message about grace ban (Encoded string)
 */
@property (nonatomic, strong) NSString *message;

/**
 Current payment status
 */
@property (nonatomic, strong, nullable) TCGBPaymentStatus *paymentStatus;

/**
 Payment condition to release ban.
 */
@property (nonatomic, strong, nullable) TCGBReleaseRuleCondition *releaseRuleCondition;

+ (instancetype)graceBanInfoWithDictionary:(NSDictionary *)dictionary;
+ (instancetype)graceBanInfoWithJSONString:(NSString *)jsonString;

- (NSString *)jsonString;
- (NSString *)prettyJsonString;
- (NSDictionary *)dictionary;

@end

/** The TCGBPaymentStatus class have information about current payment status.
 */
@interface TCGBPaymentStatus : NSObject<TCGBValueObject>

/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/**
 Amount paid
 */
@property (nonatomic, assign) double amount;

/**
 Number of payment
 */
@property (nonatomic, assign) int count;

/**
 Currency
 */
@property (nonatomic, strong) NSString *currency;

+ (instancetype)paymentStatusWithDictionary:(NSDictionary *)dictionary;
+ (instancetype)paymentStatusWithJSONString:(NSString *)jsonString;

- (NSString *)jsonString;
- (NSString *)prettyJsonString;
- (NSDictionary *)dictionary;

@end

/** The TCGBReleaseRuleCondition class have information about payment condition to release ban.
 */
@interface TCGBReleaseRuleCondition : NSObject<TCGBValueObject>

/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/**
 Amount to be paid
 */
@property (nonatomic, assign) double amount;

/**
 Number of payment required
 */
@property (nonatomic, assign) int count;

/**
 Currency
 */
@property (nonatomic, strong) NSString *currency;

/**
 Condition for amount and count ("AND" / "OR")
 */
@property (nonatomic, strong) NSString *conditionType;

+ (instancetype)releaseRuleConditionWithDictionary:(NSDictionary *)dictionary;
+ (instancetype)releaseRuleConditionWithJSONString:(NSString *)jsonString;

- (NSString *)jsonString;
- (NSString *)prettyJsonString;
- (NSDictionary *)dictionary;

@end

NS_ASSUME_NONNULL_END
