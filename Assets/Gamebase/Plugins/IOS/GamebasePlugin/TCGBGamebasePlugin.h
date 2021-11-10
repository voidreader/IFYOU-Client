#import <Foundation/Foundation.h>
#import <Gamebase/Gamebase.h>
#import "TCGBPluginData.h"

typedef void (^ServerPushMessage)(TCGBServerPushMessage*);
typedef void (^ObserverMessage)(TCGBObserverMessage*);
typedef void (^EventHandlerMessage)(TCGBGamebaseEventMessage*);

@interface TCGBGamebasePlugin : NSObject {
    ServerPushMessage serverPushMessage;
    ObserverMessage observerMessage;
    EventHandlerMessage eventHandlerMessage;
    TCGBPluginData* observerPluginData;
    TCGBPluginData* serverPushEventPluginData;
    TCGBPluginData* eventHandlerPluginData;
}

@property (nonatomic, strong) ObserverMessage observerMessage;
@property (nonatomic, strong) TCGBPluginData* observerPluginData;

@property (nonatomic, strong) ServerPushMessage serverPushMessage;
@property (nonatomic, strong) TCGBPluginData* serverPushEventPluginData;

@property (nonatomic, strong) EventHandlerMessage eventHandlerMessage;
@property (nonatomic, strong) TCGBPluginData* eventHandlerPluginData;
@end
