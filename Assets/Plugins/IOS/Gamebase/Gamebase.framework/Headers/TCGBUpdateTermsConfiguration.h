//
//  TCGBUpdateTermsConfiguration.h
//  Gamebase
//
//  Created by NHN on 2021/01/13.
//  Copyright © 2021 NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "TCGBValueObject.h"

@class TCGBTermsContent;

NS_ASSUME_NONNULL_BEGIN

@interface TCGBUpdateTermsConfiguration : NSObject

/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/**
 Terms version
 */
@property (nonatomic, strong) NSString *termsVersion;

/**
 Terms key
 */
@property (nonatomic, assign) int termsSeq;

/**
 Terms item detail information
 */
@property (nonatomic, strong) NSArray<TCGBTermsContent *> *contents;


/**---------------------------------------------------------------------------------------
 * @name Initialization
 *  ---------------------------------------------------------------------------------------
 */

/**
 Creates a TCGBUpdateConfiguration instance with several properties.
 
 @param termsVersion Terms version
 @param termsSeq Terms key
 @param contents Terms item detail information
 */
+ (instancetype)updateTermsConfigurationWithTermsVersion:(NSString *)termsVersion termsSeq:(int)termsSeq contents:(NSArray<TCGBTermsContent *> *)contents;

- (NSString *)jsonString;
- (NSString *)prettyJsonString;

@end

NS_ASSUME_NONNULL_END
