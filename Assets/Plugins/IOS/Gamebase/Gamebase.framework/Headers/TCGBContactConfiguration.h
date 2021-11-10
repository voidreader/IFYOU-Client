//
//  TCGBContactConfiguration.h
//  Gamebase
//
//  Created by NHN on 2020/09/01.
//  Copyright © 2020 NHN Corp. All rights reserved.
//

#ifndef TCGBContactConfiguration_h
#define TCGBContactConfiguration_h

#import <Foundation/Foundation.h>
#import "TCGBValueObject.h"

NS_ASSUME_NONNULL_BEGIN

@interface TCGBContactConfiguration : NSObject <TCGBValueObject>

/**
This value represents the custom userName.
*/
@property (nonatomic, strong, nullable) NSString *userName;
@property (nonatomic, strong, nullable) NSDictionary<NSString *, NSString *> *extraData;
@property (nonatomic, strong, nullable) NSString *additionalURL;


+ (instancetype)contactConfigurationWithUserName:(NSString *)userName;
+ (instancetype)contactConfigurationWithUserName:(NSString *)userName extraData:(NSDictionary<NSString *, NSString *> *)extraData;
+ (instancetype)contactConfigurationWithAdditionalURL:(NSString *)additionalURL;
+ (nullable instancetype)contactConfigurationWithJSONString:(NSString *)jsonString;
- (NSString *)jsonString;
- (NSString *)prettyJsonString;


@end

NS_ASSUME_NONNULL_END

#endif /* TCGBContactConfiguration_h */
