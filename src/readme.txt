1.Folders and files structure. Please use this approach:
https://scotch.io/tutorials/angularjs-best-practices-directory-structure#a-better-structure-and-foundation

Example:

	app/
	----- shared/   // acts as reusable components or partials of our site
	---------- sidebar/
	--------------- sidebarDirective.js
	--------------- sidebarView.html
	---------- article/
	--------------- articleDirective.js
	--------------- articleView.html
	----- components/   // each component is treated as a mini Angular app
	---------- home/
	--------------- homeController.js
	--------------- homeService.js
	--------------- homeView.html
	---------- blog/
	--------------- blogController.js
	--------------- blogService.js
	--------------- blogView.html
	----- app.module.js
	----- app.routes.js

2. Handling js dependecies:
	* Add dependecy to bower.json
	* In case order of dependencies is important, write correct order in gulpfile.js file 

3. Resolve issue for windows path max length:
npm install -g flatten-packages
cd PROJECT_DIRECTORY
flatten-packages