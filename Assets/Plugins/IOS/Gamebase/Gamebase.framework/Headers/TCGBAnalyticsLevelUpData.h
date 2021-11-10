//
//  TCGBAnalyticsLevelUpData.h
//  Gamebase
//
//  Created by NHN on 21/11/2018.
//  © NHN Corp. All rights reserved.
//
#import <Foundation/Foundation.h>

#ifndef TCGBAnalyticsLevelUpData_h
#define TCGBAnalyticsLevelUpData_h

NS_ASSUME_NONNULL_BEGIN

@interface TCGBAnalyticsLevelUpData : NSObject

// Property
@property (nonatomic, assign) int userLevel; // Required.
@property (nonatomic, assign) long long levelUpTime; // Required. epochTime in millis. Accept only positive values.


// Initializer
- (instancetype)init __attribute__((unavailable("Must use initWithUserLevel:levelUpTime: instead.")));
- (instancetype)initWithUserLevel:(int)userLevel levelUpTime:(long long)levelUpTime;;
+ (instancetype)levelUpDataWithUserLevel:(int)userLevel levelUpTime:(long long)levelUpTime;

// for iOS Only
- (void)setLevelUpTimeWithDate:(NSDate *)now; // Convert NSDate to epoch time and set levelUpTime.

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBAnalyticsLevelUpData_h */
