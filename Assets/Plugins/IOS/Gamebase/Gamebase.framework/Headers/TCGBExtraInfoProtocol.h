//
//  TCGBExtraInfoProtocol.h
//  Gamebase
//
//  Created by NHNEnt on 2020/02/25.
//  Copyright © 2020 NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@protocol TCGBExtraInfoProtocol <NSObject>

@required


@optional
- (nullable NSString *)jsonString;
- (nullable NSString *)prettyJsonString;
- (void)objectWithDictionary:(NSDictionary *)dictionary;

@end

NS_ASSUME_NONNULL_END
