//
//  TCGBToastLog.h
//  Gamebase
//
//  Created by NHNEnt on 03/06/2019.
//  Copyright © 2019 NHN Corp. All rights reserved.
//

#ifndef TCGBToastLog_h
#define TCGBToastLog_h

#import <Foundation/Foundation.h>
#import "TCGBLoggerConstants.h"

NS_ASSUME_NONNULL_BEGIN

@interface TCGBLog : NSObject

- (NSString *)transactionID;
- (TCGBLoggerLevel)level;
- (NSString *)type;
- (NSString *)message;
- (NSNumber *)createTime;
- (nullable NSDictionary<NSString *, NSString *> *)userFields;

+ (instancetype)logWithLevel:(TCGBLoggerLevel)level
                        type:(NSString *)type
                     message:(NSString *)message
                  userFields:(nullable NSDictionary<NSString *, NSString *> *)userFields;

- (instancetype)initWithLevel:(TCGBLoggerLevel)level
                         type:(NSString *)type
                      message:(NSString *)message
                   userFields:(nullable NSDictionary<NSString *, NSString *> *)userFields;

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBToastLog_h */
