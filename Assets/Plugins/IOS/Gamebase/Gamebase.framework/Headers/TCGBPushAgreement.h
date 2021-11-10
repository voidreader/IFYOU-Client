//
//  TCGBPushAgreement.h
//  Gamebase
//
//  Created by NHN on 2020/08/10.
//  Copyright © 2020 NHN Corp. All rights reserved.
//

#ifndef TCGBPushAgreement_h
#define TCGBPushAgreement_h

#import <Foundation/Foundation.h>
#import "TCGBValueObject.h"

NS_ASSUME_NONNULL_BEGIN

@interface TCGBPushAgreement : NSObject <TCGBValueObject>

@property (nonatomic, assign) BOOL pushEnabled;
@property (nonatomic, assign) BOOL ADAgreement;
@property (nonatomic, assign) BOOL ADAgreementNight;


+ (TCGBPushAgreement *)pushAgreementWithPushEnabled:(BOOL)pushEnabled ADAgreement:(BOOL)ADAgreement ADAgreementNight:(BOOL)ADAgreementNight;
+ (TCGBPushAgreement *)pushAgreementWithJSONString:(NSString *)jsonString;

#pragma mark - Utility
- (NSString *)jsonString;
- (NSString *)prettyJsonString;

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBPushAgreement_h */
