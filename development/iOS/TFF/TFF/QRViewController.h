//
//  QRViewController.h
//  TFF
//
//  Created by Cristian Asenjo on 2013-02-25.
//  Copyright (c) 2013 CloudNine. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface QRViewController : UIViewController<ZBarReaderDelegate>

- (BOOL) isValidTFFURL:(NSString *) scannedURL;
- (void) showAlert :(NSString *)title :(NSString *)message;
@property (nonatomic, strong) NSString *scannedCodeData;
@property (nonatomic, strong) ZBarReaderViewController *reader;
@end