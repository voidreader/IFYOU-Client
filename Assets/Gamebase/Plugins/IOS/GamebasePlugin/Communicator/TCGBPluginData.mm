#import <Foundation/Foundation.h>
#import "TCGBPluginData.h"

@implementation TCGBPluginData

@synthesize jsonData = _jsonData;
@synthesize completion = _completion;

-(id)initWithJsonData:(NSString*)jsonData completion:(SendCompletion)completion {
    if(self = [super init]) {
        self.jsonData = jsonData;
        self.completion = completion;
    }
    return self;
}

@end
