//
//  TCGBTermsContent.h
//  Gamebase
//
//  Created by NHN on 2021/01/11.
//  Copyright © 2021 NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "TCGBValueObject.h"

NS_ASSUME_NONNULL_BEGIN

@interface TCGBTermsContent : NSObject <TCGBValueObject>

@property (nonatomic, assign) int termsContentSeq;
@property (nonatomic, assign) bool agreed;

+ (instancetype)termsContentWithTermsContentSeq:(int)termsContentSeq agreed:(bool)agreed;
+ (instancetype)termsContentWithDictionary:(NSDictionary *)dictionary;
- (NSString *)jsonString;
- (NSString *)prettyJsonString;
- (NSDictionary *)dictionaryWithContent;

@end

NS_ASSUME_NONNULL_END
