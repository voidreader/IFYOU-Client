//
//  TCGBUpdateInfo.h
//  Gamebase
//
//  Created by NHNEnt on 2020/02/25.
//  Copyright © 2020 NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "TCGBExtraInfoProtocol.h"
#import "TCGBError.h"
#import "TCGBError+Ex.h"

NS_ASSUME_NONNULL_BEGIN

extern NSString* const kTCGBUpdateInfoKeyname;
extern NSString* const kTCGBUpdateInfoInstallUrlKeyname;
extern NSString* const kTCGBUpdateInfoMessageKeyname;

@interface TCGBUpdateInfo : NSObject <TCGBExtraInfoProtocol>

@property (nonatomic, strong, nullable) NSString* installUrl;
@property (nonatomic, strong, nullable) NSString* message;

+ (nullable TCGBUpdateInfo *)updateInfoFromError:(TCGBError *)error;
+ (nullable TCGBUpdateInfo *)updateInfoFromDictionary:(NSDictionary *)dic;

- (instancetype)init __attribute__((unavailable("init not available.")));
- (instancetype)initFromError:(TCGBError *)error;
@end

NS_ASSUME_NONNULL_END
