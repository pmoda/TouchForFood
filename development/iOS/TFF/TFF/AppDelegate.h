//
//  AppDelegate.h
//  TFF
//
//  Created by Cristian Asenjo on 2013-01-29.
//  Copyright (c) 2013 CloudNine. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface AppDelegate : UIResponder <UIApplicationDelegate>

@property (strong, nonatomic) UIWindow *window;
@property (nonatomic, retain) NSString *scannedURL;
@property (nonatomic) BOOL codeAlreadyScanned;

@end
