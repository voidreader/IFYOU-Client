//
//  TCGBPushConfiguration.h
//  Gamebase
//
//  Created by NHNEnt on 28/08/2019.
//  Copyright © 2019 NHN Corp. All rights reserved.
//

#ifndef TCGBPushConfiguration_h
#define TCGBPushConfiguration_h

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@class TCGBDataContainer;

/** The TCGBPushConfiguration class configures the behavior of TCGBPush.
 */
@interface TCGBPushConfiguration : NSObject

/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/**
 Enable push
 */
@property (nonatomic, assign) BOOL pushEnabled;

/**
 Agreement of getting advertising push
 */
@property (nonatomic, assign) BOOL ADAgreement;

/**
 Agreement of getting advertising push at night
 */
@property (nonatomic, assign) BOOL ADAgreementNight;

/**
 Setting language of Push Message
 */
@property (nonatomic, strong, nullable) NSString* displayLanguageCode;

/**---------------------------------------------------------------------------------------
 * @name Initialization
 *  ---------------------------------------------------------------------------------------
 */

/**
 Creates a TCGBPushConfiguration instance with several properties.
 
 @param enable whether push enable or not.
 @param ADAgree `YES` if user agree on advertising push notification.
 @param ADAgreeNight `YES` if user agree on getting advertising push notification at night.
 */
+ (instancetype)pushConfigurationWithPushEnable:(BOOL)enable ADAgreement:(BOOL)ADAgree ADAgreementNight:(BOOL)ADAgreeNight;

/**
 Creates a TCGBPushConfiguration instance with several properties.
 
 @param enable whether push enable or not.
 @param ADAgree `YES` if user agree on advertising push notification.
 @param ADAgreeNight `YES` if user agree on getting advertising push notification at night.
 @param displayLanguage set language code for Push. (kTCPushKeyLanguage in TOAST Push SDK)
 */
+ (instancetype)pushConfigurationWithPushEnable:(BOOL)enable ADAgreement:(BOOL)ADAgree ADAgreementNight:(BOOL)ADAgreeNight displayLanguage:(nullable NSString *)displayLanguage;

/**
 Creates a TCGBPushConfiguration instance with several properties.
 @param jsonString In this string, there should be `pushEnabled`, `adAgreement`, `adAgreementNight` keys in the json formatted string.
 */
+ (instancetype)pushConfigurationWithJSONString:(NSString *)jsonString;

+ (nullable instancetype)fromDataContainer:(TCGBDataContainer *)dataContainer;

- (NSString *)JSONString;
- (NSString *)JSONPrettyString;

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBPushConfiguration_h */
	
