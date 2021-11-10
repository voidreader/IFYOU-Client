//
//  TCGBError+Ex.h
//  Gamebase
//
//  Created by NHN on 13/02/2019.
//  © NHN Corp. All rights reserved.
//

#ifndef TCGBError_Ex_h
#define TCGBError_Ex_h

#import "TCGBError.h"

NS_ASSUME_NONNULL_BEGIN

@interface TCGBError()

// FIXME
@property (nonatomic, strong, nullable) NSDictionary* extras; // Value must be a json string.

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBError_Ex_h */
