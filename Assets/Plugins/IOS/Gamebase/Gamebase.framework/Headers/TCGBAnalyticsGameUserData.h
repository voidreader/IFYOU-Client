//
//  TCGBAnalyticsGameUserData.h
//  Gamebase
//
//  Created by NHN on 21/11/2018.
//  © NHN Corp. All rights reserved.
//
#import <Foundation/Foundation.h>
#import "TCGBValueObject.h"

#ifndef TCGBAnalyticsGameUserData_h
#define TCGBAnalyticsGameUserData_h

NS_ASSUME_NONNULL_BEGIN

@interface TCGBAnalyticsGameUserData : NSObject <TCGBValueObject>

// Property
@property (nonatomic, assign) int userLevel; // Required.
@property (nonatomic, strong, nullable) NSString* channelId; // Optional. Default value is nil. Accept only positive values.
@property (nonatomic, strong, nullable) NSString* characterId; // Optional. Default value is nil.
@property (nonatomic, strong, nullable) NSString* classId; // Optional. Default value is nil.

// Initializer
- (instancetype)init __attribute__((unavailable("Must use initWithUserLevel: instead.")));
- (instancetype)initWithUserLevel:(int)userLevel;
+ (instancetype)gameUserDataWithUserLevel:(int)userLevel;

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBAnalyticsGameUserData_h */
