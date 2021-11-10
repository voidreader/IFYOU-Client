//
//  TCGBSystemInfo.h
//  Gamebase
//
//  Created by NHN on 2016. 6. 30..
//  © NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface TCGBSystemInfo : NSObject

+ (nullable NSString *)zoneType;
+ (NSString *)getUUID;
+ (NSString *)UDID;
+ (NSString *)ADID;
+ (NSString *)carrierCode;
+ (NSString *)carrierName;
+ (NSString *)countryCode;
+ (NSString *)languageCode;

@end

NS_ASSUME_NONNULL_END
