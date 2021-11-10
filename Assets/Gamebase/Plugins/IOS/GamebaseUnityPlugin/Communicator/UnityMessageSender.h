#import <Foundation/Foundation.h>
#import "NativeMessage.h"

@interface UnityMessageSender: NSObject {
    
}

+(UnityMessageSender*)sharedUnityMessageSender;

-(void)sendMessage:(NativeMessage*)message gameObjectName:(NSString*)gameObjectName responseMethodName :(NSString*)responseMethodName;

@end
