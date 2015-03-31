# Portable Data Annotations
A portable implementation of Data Annotations using the PCL bait and switch technique.

## Overview

DataAnnotations is an excellent library providing validation support in .NET. However, it is not implemented on the Windows Phone platforms (Silverlight and WinRT). When creating a PCL using those platforms, DataAnnotations will be unavailable.

This library resolves that issue using the [PCL Bait and Switch technique](http://log.paulbetts.org/the-bait-and-switch-pcl-trick/). It features a complete implementation of the DataAnnotations library for the Windows Phones platforms, with bait and switch used for the other platforms. What does this mean? It means that on platforms where DataAnnotations is supported, it will work as expected. But on platforms where it is not supported, it will now be available to you. You can write your PCLs targeting DataAnnotations and know that it will just work on whatever platform you deploy to.

## How Does It Work?

All platforms that support DataAnnotations will use the TypeForwardedToAttribute to forward their implementations to the correct classes at runtime. On other platforms (Windows Phone), a custom implementation matching the same surface area will be used. At compile time, you write your code to the portable API and at runtime it runs on the native API - or on the portable API if no native API is present (again, only on Windows Phone).

Using this forwarding technique means that Microsoft or 3rd Party APIs that build on DataAnnotations will run correctly.

## What is the Surface Area of the API?

I started by providing the same surface area as the DataAnnotations classes that are available in a normal PCL (without Windows Phone). This is also the same as the Silverlight 5 surface area. Then I added some of the newer classes that are available in the .NET 4.5 API, so that the final surface area is larger than the normal PCL surface area.

There are only five classes from the .NET 4.5.2 DataAnnotations library that are not present in this library. They are:

`AssociatedMetadataTypeTypeDescriptionProvider`: Reflection support necessary to make this class work is not present in PCLs. It is not a very useful class anyway.
`CompareAttribute`: There are non-portable elements in the implementation. I did not want to figure out if/how I can work around those yet. This class may be added in the future.
`FileExtensionsAttribute`: Same as above. Non-portable elements, may be added in the future.
`ScaffoldColumnAttribute`: I believe this was just overlooked and I can add this back in the future.
`ScaffoldTableAttribute`: Same as above - I think I overlooked this and can add it back in the future.

On many platforms, only a subset of DataAnnotations matching the Silverlight and PCL surface areas is available. For those platforms, I have backported the missing classes to increase the overall surface area of the library. When using those classes, a custom implementation will be used instead of a TypeForwardedToAttribute. This should not matter since these classes were unavailable on those platforms anyway.

## Limitations

The DataType enumeration is already implemented on some platforms, but with less enumeration values than the ones in .NET 4.5.2. Because of that, I have only exposed the smaller surface area to the PCL. The new values supported in .NET 4.5.2 (`CreditCard`, `PostalCode`, `Upload`) are thus not available. This also means that on platforms where `CreditCardAttribute` is not present, and thus implemented in the library, the DataType will report as `Custom` rather than the expected, but unavailable, `CreditCard`. On platforms where it is supported (.NET 4.5+), it will return the correct value `CreditCard`. This inconsistency is unfortunate, but totally acceptable and I advise userse not to rely on the value of the DataType property in your own code.

Also, though I said the library uses TypeForwardedToAttribute in all cases where possible, technically that is not true. In the case of Xamarin Android/iOS/Mac, I chose to use one project for simplicity. The full .NET 4.5 DataAnnotations is available in Mono, but only if you choose a specific platform project, such as an Android project, an iOS project, etc. If you choose a PCL that supports all of them, it reduces the surface area to the normal PCL surface area. In that case, I add back custom versions of the missing classes. The alternative is to create specific projects for each Xamarin platform. I chose not to do this because 1) I do not see a need because Android/iOS/Mac have no built in support for DataAnnotations anyway, and therefore this will cause no loss of functionality, and 2) extra projects means extra maintenance, and in this case also means necessitating a Mac build host for building the iOS and Mac projects. Since there is nothing to gain, I have chosen not to do this.

## Unit Testing

There is a limitation to unit testing that may be difficult to understand if you are not aware. Unit testing frameworks are not aware of the bait and switch technique, and can therefore cause failing tests for unclear reasons. If you are testing a PCL that uses this library and wish to unit test the validation logic, you may run into a (resolvable) issue. If you derive a class in your unit testing project and put a `ValidationAttribute` on it, then run code from your PCL that calls `Validator.Validate()`, it will fail. This is because, not knowing about bait and switch, the unit test runner will load the PCL version of the library for the PCL and the .NET version of the library for the unit test project. Thus, your call to `Validate()` will not find the correct attribute class. The reverse will also cause a problem (calling `Validate()` in the .NET test project on attributes defined in the PCL). The workaround for this is not to test the library in this manner. All the code for validation should be in your PCL and invoked from your test project without needing any references to DataAnnotations in your test project. If you do this, your tests will succeed as expected. I assume you can run into similar problems if your code under tests spans multiple projects and platforms. Avoid this scenario where possible.

Keep in mind as well that if you do unit test the validation in PCLs, you will be unit testing against the PCL implementations, not the implementations actually used at runtime. This implementation is only used by Windows Phone, so your tests are really only valid on that platform. However, you should not need to unit test .NET Framework features anyway, so there is really no need to validate that the validation works.

## Supported Platforms

.NET 4.5/4.0, iOS, Android, Windows Store 8.0/8.1, Windows Phone Silverlight 8.1/8.0/7.5, Windows Phone 8.1, Silverlight 5.0, and all portable libraries including those platforms.
