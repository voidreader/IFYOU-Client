//
//  TCGBTransferAccountFailInfo.h
//  Gamebase
//
//  Created by NHN on 07/02/2019.
//  © NHN Corp. All rights reserved.
//
#import <Foundation/Foundation.h>
#import "TCGBError.h"

#ifndef TCGBTransferAccountFailInfo_h
#define TCGBTransferAccountFailInfo_h

NS_ASSUME_NONNULL_BEGIN

@interface TCGBTransferAccountFailInfo : NSObject

@property (nonatomic, strong, readonly) NSString* appId;
@property (nonatomic, strong, readonly) NSString* accountId;
@property (nonatomic, strong, readonly) NSString* status;
@property (nonatomic, assign, readonly) NSInteger failCount;
@property (nonatomic, assign, readonly) long long blockEndDate;
@property (nonatomic, assign, readonly) long long regDate;

- (instancetype)init __attribute__((unavailable("init not available.")));
+ (nullable TCGBTransferAccountFailInfo *)transferAccountFailInfoFromError:(TCGBError *)error;

@end

@interface TCGBTransferAccountFailInfo (deprecated)
+ (nullable TCGBTransferAccountFailInfo *)resultWithTCGBError:(TCGBError *)error DEPRECATED_MSG_ATTRIBUTE("Use resultFromError: method instead.");
@end

NS_ASSUME_NONNULL_END

#endif /* TCGBTransferAccountFailInfo_h */
