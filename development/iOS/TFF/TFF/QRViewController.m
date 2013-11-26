//
//  QRViewController.m
//  TFF
//
//  Created by Cristian Asenjo on 2013-02-25.
//  Copyright (c) 2013 CloudNine. All rights reserved.
//

#import "QRViewController.h"
#import "AppDelegate.h"

@interface QRViewController ()

@end

@implementation QRViewController

@synthesize scannedCodeData;
@synthesize reader;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:YES];
    
    AppDelegate *delegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    if(delegate.codeAlreadyScanned)
    {
        [self showAlert:@"Code already scanned" :@"You already scanned a TFF code. Please switch to the TFF tab to order your food."];
    }
    else
    {
        // ADD: present a barcode reader that scans from the camera feed
        reader = [ZBarReaderViewController new];
        reader.readerDelegate = self;
        reader.supportedOrientationsMask = ZBarOrientationMaskAll;
        
        
        ZBarImageScanner *scanner = reader.scanner;
        // Reader configuration below. We disable all symbologies except for QR codes
        [scanner setSymbology: 0
                       config: ZBAR_CFG_ENABLE
                           to: 0];
        [scanner setSymbology: ZBAR_QRCODE
                       config: ZBAR_CFG_ENABLE
                           to: 1];
        reader.readerView.zoom = 1.0;
        
        // Present and release the controller
        [self presentViewController:reader animated:YES completion:nil];
    }


}

// This method fires automatically when the image scanned is a valid QR code
- (void) imagePickerController: (UIImagePickerController*) reader
 didFinishPickingMediaWithInfo: (NSDictionary*) info
{
     NSLog(@"The image picker is calling successfully %@",info);
    
    // Get the decoded results
    id<NSFastEnumeration> results = [info objectForKey: ZBarReaderControllerResults];
    ZBarSymbol *symbol = nil;
    
    // Just grab the first barcode
    for(symbol in results) {
        scannedCodeData = [NSString stringWithString:symbol.data];
        NSLog(@"The symbols  is the following %@", scannedCodeData);
        break;
    }
    
    if([self isValidTFFURL:scannedCodeData])
    {
        [reader dismissViewControllerAnimated:NO completion:nil];
        AppDelegate *delegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
        delegate.scannedURL = scannedCodeData;
        delegate.codeAlreadyScanned = YES;
        self.tabBarController.selectedViewController = [self.tabBarController.viewControllers objectAtIndex:1];
    }
}

- (BOOL) isValidTFFURL:(NSString *) scannedURL
{
    NSString *strRegEx = @"^(http://ryanweb\.dyndns\.info:2431){1}.*$";
    
    NSRegularExpression *regex = [NSRegularExpression
                                  regularExpressionWithPattern:strRegEx
                                  options:0
                                  error:NULL];
    
    NSUInteger numberOfMatches = [regex numberOfMatchesInString:scannedURL
                                                        options:0
                                                          range:NSMakeRange(0, [scannedURL length])];
    
    
    if(numberOfMatches > 0)
    {
        NSLog(@"URL is valid");
        return true;
    }
    else
    {
        NSLog(@"URL is invalid");
        [self showAlert:@"Invalid QR Code":@"This is not a valid TouchForFood QR code. Please try again."];
        return false;
    }
}

- (void) showAlert:(NSString *)title :(NSString *)message {
    
    UIAlertView *alert = [[UIAlertView alloc]
                          
                          initWithTitle:title
                          message:message
                          delegate:nil
                          cancelButtonTitle:@"OK"
                          otherButtonTitles:nil];
    
    [alert show];
}

// This method fires if we hit the cancel button on the QR scanner view
- (void) imagePickerControllerDidCancel:(UIImagePickerController*)picker
{
    AppDelegate *delegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    if(!delegate.codeAlreadyScanned)
    {
        [self showAlert:@"QR Code not scanned":@"You did not scan a valid QR code. Please scan a valid QR code to continue"];
        [self viewWillAppear:YES];
    }
    else
    {
        [reader dismissViewControllerAnimated:NO completion:nil];
    }

}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

@end
