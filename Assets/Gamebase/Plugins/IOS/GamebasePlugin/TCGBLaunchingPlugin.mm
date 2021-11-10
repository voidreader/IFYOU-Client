#import "TCGBLaunchingPlugin.h"
#import "DelegateManager.h"
#import <Gamebase/Gamebase.h>
#import "TCGBJsonUtil.h"
#import "TCGBPluginData.h"

#define NLAUNCHING_API_GET_LAUNCHING_INFORMATIONS       @"gamebase://getLaunchingInformations"
#define NLAUNCHING_API_GET_LAUNCHING_STATUS             @"gamebase://getLaunchingStatus"

@implementation TCGBLaunchingPlugin

- (instancetype)init {
    if ((self = [super init]) == nil) {
        return nil;
    }
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:NLAUNCHING_API_GET_LAUNCHING_INFORMATIONS target:self selector:@selector(getLaunchingInformations:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:NLAUNCHING_API_GET_LAUNCHING_STATUS target:self selector:@selector(getLaunchingStatus:)];
    
    return self;
}

-(NSString*)getLaunchingInformations:(TCGBPluginData*)pluginData {
    return [[TCGBLaunching launchingInformations] JSONString];
}

-(NSString*)getLaunchingStatus:(TCGBPluginData*)pluginData {
    NSString* result = [@([TCGBLaunching launchingStatus]) stringValue];
    return result;
}
@end

