//
//  TCGBBanInfo.h
//  Gamebase
//
//  Created by NHN on 2017. 9. 4..
//  © NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "TCGBError.h"

NS_ASSUME_NONNULL_BEGIN

@interface TCGBBanInfo : NSObject
@property (nonatomic, strong) NSString*     userId;
@property (nonatomic, strong) NSString*     banType;
@property (nonatomic, strong) NSNumber*     beginDate;
@property (nonatomic, strong) NSNumber*     endDate;
@property (nonatomic, strong) NSString*     message;
@property (nonatomic, strong, nullable) NSString*     csInfo;
@property (nonatomic, strong, nullable) NSString*     csUrl;

+ (nullable TCGBBanInfo *)banInfoFromError:(TCGBError *)error;
- (NSDictionary *)dictionaryOfBanInfo;

@end

NS_ASSUME_NONNULL_END
