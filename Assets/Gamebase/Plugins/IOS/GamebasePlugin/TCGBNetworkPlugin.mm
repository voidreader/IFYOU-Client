#import "TCGBNetworkPlugin.h"
#import "DelegateManager.h"
#import <Gamebase/Gamebase.h>
#import "TCGBJsonUtil.h"
#import "TCGBPluginData.h"

#define NETWORK_API_GET_TYPE                            @"gamebase://getType"
#define NETWORK_API_GET_TYPE_NAME                       @"gamebase://getTypeName"
#define NETWORK_API_IS_CONNECTED                        @"gamebase://isConnected"

@implementation TCGBNetworkPlugin

- (instancetype)init {
    if ((self = [super init]) == nil) {
        return nil;
    }
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:NETWORK_API_GET_TYPE target:self selector:@selector(getType:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:NETWORK_API_GET_TYPE_NAME target:self selector:@selector(getTypeName:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:NETWORK_API_IS_CONNECTED target:self selector:@selector(isConnected:)];
    
    return self;
}

-(NSString*)getType:(TCGBPluginData*)pluginData {
    NSString* result = [@([TCGBNetwork type]) stringValue];
    return result;
}

-(NSString*)getTypeName:(TCGBPluginData*)pluginData {
    NSString* result = [TCGBNetwork typeName];
    return result;
}

-(NSString*)isConnected:(TCGBPluginData*)pluginData {
    NSMutableDictionary *contentDictionary = [[NSMutableDictionary alloc]init];
    [contentDictionary setValue:[NSNumber numberWithBool:[TCGBNetwork isConnected]] forKey:@"isConnected"];
    
    NSString* result = [contentDictionary JSONString];
    if(result == nil)
        return @"";
    
    return result;
}
@end

