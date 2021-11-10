//
//  TCGBDataContainer.h
//  Gamebase
//
//  Created by NHN on 2021/01/07.
//  Copyright © 2021 NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "TCGBValueObject.h"

NS_ASSUME_NONNULL_BEGIN

@interface TCGBDataContainer : NSObject <TCGBValueObject>

@property (nonatomic, strong) NSString *data;

+ (instancetype)dataContainerWithData:(NSString *)data;

- (NSString *)description;
- (NSString *)debugDescription;
- (NSString *)jsonString;
- (NSString *)prettyJsonString;

@end

NS_ASSUME_NONNULL_END
