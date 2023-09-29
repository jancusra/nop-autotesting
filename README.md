# NopCommerce plugin for functional tests (version 4.60)

You can define sets of pages in administration and group them to the task. Every page has group of commands (etc. click to object, fill input, wait for ajax to complete, report if element exist and more ...)  
If task is defined in administration, can be used after an every deploy to check if everything is working fine (etc. product page and catalog are showed, products are added to the cart, checkout is OK and more ....)  
Task can run on background and at the end will show final report by defined criteria.  
Plugin is available with SQL script for some sample data after a clean NopCommerce installation, so you can try him and study without much effort!  
Principal functionality by injecting javascript and automatic tests by this plugin can be used for any project.  

  
**Quick guide how to:**
1. download and install NopCommerce, copy main plugin folder to /src/Plugins
2. start NopCommerce from source code and install this plugin
3. run script on database from folder here /KSystem.Nop.Plugin.Misc.AutoTesting/Sql/nop_splitstring_to_table.sql for some sample data
4. now you can go to administration and run defined task (see live video preview below)
[![LIVE PREVIEW](https://img.youtube.com/vi/z-wg3fwAMlU/0.jpg)](https://www.youtube.com/watch?v=z-wg3fwAMlU)
