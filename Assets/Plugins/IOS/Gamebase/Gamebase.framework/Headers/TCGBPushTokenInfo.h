//
//  TCGBPushTokenInfo.h
//  Gamebase
//
//  Created by NHN on 2020/08/07.
//  Copyright © 2020 NHN Corp. All rights reserved.
//

#ifndef TCGBPushTokenInfo_h
#define TCGBPushTokenInfo_h

#import <Foundation/Foundation.h>
#import "TCGBValueObject.h"

NS_ASSUME_NONNULL_BEGIN

@class TCGBPushAgreement;

@interface TCGBPushTokenInfo : NSObject <TCGBValueObject>

@property (nonatomic, strong)                 NSString *pushType;
@property (nonatomic, strong)                 NSString *token;
@property (nonatomic, strong)                 NSString *userId;
@property (nonatomic, strong)                 NSString *deviceCountryCode;
@property (nonatomic, strong)                 NSString *timezone;
@property (nonatomic, strong)                 NSString *registeredDateTime;
@property (nonatomic, strong)                 NSString *languageCode;
@property (nonatomic, assign)                     BOOL sandbox;
@property (nonatomic, strong)        TCGBPushAgreement *agreement;

+ (TCGBPushTokenInfo *)pushTokenInfoWithPushType:(NSString *)pushType token:(NSString *)token userId:(NSString *)userId deviceCountryCode:(NSString *)deviceCountryCode timezone:(NSString *)timezone registeredDateTime:(NSString *)registeredDateTime languageCode:(NSString *)languageCode sandbox:(BOOL)sandbox agreement:(TCGBPushAgreement *)agreement;

+ (TCGBPushTokenInfo *)pushTokenInfoWithJSONString:(NSString *)jsonString;

#pragma mark - Utility
- (NSString *)jsonString;
- (NSString *)prettyJsonString;

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBPushTokenInfo_h */
