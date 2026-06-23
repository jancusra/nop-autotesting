# NopCommerce plugin for functional tests (v. 4.60)
#### [ FULLY FUNCTIONAL, BUT NOT MAINTAINED CODE (ONLY FOR DEMONSTRATION) ]

You can define sets of pages in the administration area and group them into a task. Every page has a group of commands (e.g. click an object, fill an input, wait for AJAX to complete, report whether an element exists and more ...)  
Once a task is defined in the administration area, it can be used after every deploy to check whether everything is working fine (e.g. the product page and catalog are shown, products are added to the cart, checkout is OK and more ....)  
The task can run in the background and, at the end, shows a final report based on the defined criteria.  
The plugin comes with an SQL script that provides sample data after a clean NopCommerce installation, so you can try it out and study it without much effort!  
The core functionality — injecting JavaScript and running automated tests with this plugin — can be used in any project.  

  
**Quick guide how to:**
1. download and install NopCommerce, then copy the main plugin folder to /src/Plugins
2. run NopCommerce from the source code and install this plugin
3. run the script /KSystem.Nop.Plugin.Misc.AutoTesting/Sql/prepare_plugin_data.sql on your database to load some sample data
4. now you can go to the administration area and run the defined task (see the live video preview below)
[![LIVE PREVIEW](https://img.youtube.com/vi/z-wg3fwAMlU/0.jpg)](https://www.youtube.com/watch?v=z-wg3fwAMlU)
