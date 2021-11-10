//
//  TCGBForcingMappingTicket.h
//  Gamebase
//
//  Created by NHN on 18/02/2019.
//  © NHN Corp. All rights reserved.
//

#ifndef TCGBForcingMappingTicket_h
#define TCGBForcingMappingTicket_h

#import <Foundation/Foundation.h>
#import "TCGBError.h"

NS_ASSUME_NONNULL_BEGIN

@interface TCGBForcingMappingTicket : NSObject

@property (nonatomic, strong, readonly) NSString* mappedUserId;
@property (nonatomic, strong, readonly) NSString* idPCode;
@property (nonatomic, strong, readonly) NSString* forcingMappingKey;
@property (nonatomic, assign, readonly) long long expirationDate;

- (instancetype)init __attribute__((unavailable("init not available.")));

+ (nullable TCGBForcingMappingTicket *)forcingMappingTicketFromError:(TCGBError *)error;

@end

@interface TCGBForcingMappingTicket (deprecated)
+ (nullable TCGBForcingMappingTicket *)forcingMappingTicketWithError:(TCGBError *)error DEPRECATED_MSG_ATTRIBUTE("Use forcingMappingTicketFromError: method instead.");

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBForcingMappingTicket_h */
