//
//  TCGBValueObject.h
//  Gamebase
//
//  Created by NHNEnt on 2020/05/14.
//  Copyright © 2020 NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@class TCGBDataContainer;

@protocol TCGBValueObject <NSObject>

@required
- (NSString *)jsonString;
- (NSString *)prettyJsonString;

@optional
- (instancetype)fromDataContainer:(TCGBDataContainer *)dataContainer;
- (instancetype)fromJsonString:(NSString *)jsonString;
- (instancetype)fromJsonObject:(NSDictionary *)jsonObject;

@end

NS_ASSUME_NONNULL_END
