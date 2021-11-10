#import <Foundation/Foundation.h>
#import "EngineMessage.h"
#import "TCGBJsonUtil.h"

@implementation EngineMessage

@synthesize scheme = _scheme;
@synthesize handle = _handle;
@synthesize jsonData = _jsonData;
@synthesize extraData = _extraData;

-(id)initWithJsonString:(NSString*)jsonString {
    if(self = [super init]) {        
        NSDictionary* convertedDic = [jsonString JSONDictionary];
        
        self.scheme = convertedDic[@"scheme"];
        self.handle = [convertedDic[@"handle"] intValue];
        self.jsonData = convertedDic[@"jsonData"];
        self.extraData = convertedDic[@"extraData"];
    }
    return self;
}
@end
