# CAML-Designer
Public repository for the CAML Designer tool

The CAML Designer is an old tool that has been developed to enable SharePoint developers to build CAML queries. It offers you following functionality:
- you can build CAML queries for single lists and document libraries
- beside the pure CAML queries, you can also get code snippets for the server-side object model, the .NET client-side object model, the JavaScript client-side object model and last but not least code snippets when working with REST.

The current version of the CAML Designer has been recompiled against .NET Framework 4.7.2. The reason why it is not compiled against .NET 6 yet, is because it doesn't support connection to O365 through username/password combination. 

You can connect to SharePoint Online, SharePoint 2019, SharePoint 2016 and SharePoint 2013 through the Client Object model. For the older versions of SharePoint you have to connect through the Web Services of SharePoint. I have no old version of SharePoint anymore, so I was not able to test that part of the code.

For SharePoint Online, connection with MFA is now supported.

The user interface is still WPF (Windows Presentation Foundation) and it deserves a fresh look. 

If you want to run the source code, set the project CamlDesigner2013 as Startup project.

You can find the documenation here: https://github.com/pnp/CAML-Designer/wiki
