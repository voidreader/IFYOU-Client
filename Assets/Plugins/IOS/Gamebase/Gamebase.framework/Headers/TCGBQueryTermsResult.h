//
//  TCGBQueryTermsResult.h
//  Gamebase
//
//  Created by NHN on 2021/01/11.
//  Copyright © 2021 NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "TCGBValueObject.h"

@class TCGBTermsContentDetail;

NS_ASSUME_NONNULL_BEGIN

@interface TCGBQueryTermsResult : NSObject

/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/**
 Terms version
 
 This property will be used in [TCGBTerms updateTermsWithViewController:configuration:completion] API
 */
@property (nonatomic, strong) NSString *termsVersion;

/**
 Terms type ("KOREAN", "GDPR", "ETC")
 */
@property (nonatomic, strong) NSString *termsCountryType;

/**
 Terms key
 
 This property will be used in [TCGBTerms updateTermsWithViewController:configuration:completion] API
 */
@property (nonatomic, assign) int termsSeq;

/**
 Terms item detail information
 
 This property will be used in [TCGBTerms updateTermsWithViewController:configuration:completion] API
 */
@property (nonatomic, strong) NSArray <TCGBTermsContentDetail *> *contents;


/**---------------------------------------------------------------------------------------
 * @name Initialization
 *  ---------------------------------------------------------------------------------------
 */

/**
 Creates a TCGBQueyTermsResult instance with several properties.
 
 @param termsVersion Terms version
 @param termsCountryType Terms type ("KOREAN", "GDPR", "ETC")
 @param termsSeq Terms key
 @param contents Terms item detail information
 */
+ (instancetype)queryTermsResultWithTermsVersion:(NSString *)termsVersion termsCountryType:(NSString *)termsCountryType termsSeq:(int)termsSeq contents:(NSArray<TCGBTermsContentDetail *> *)contents;

- (NSString *)jsonString;
- (NSString *)prettyJsonString;

@end

NS_ASSUME_NONNULL_END
