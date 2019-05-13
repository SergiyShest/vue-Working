define( [
	"../ajax"
], function( jQuery ) {

"use strict";

<<<<<<< HEAD
jQuery._evalUrl = function( url ) {
=======
jQuery._evalUrl = function( url, options ) {
>>>>>>> master
	return jQuery.ajax( {
		url: url,

		// Make this explicit, since user can override this through ajaxSetup (#11264)
		type: "GET",
		dataType: "script",
		cache: true,
		async: false,
		global: false,
<<<<<<< HEAD
		"throws": true
=======

		// Only evaluate the response if it is successful (gh-4126)
		// dataFilter is not invoked for failure responses, so using it instead
		// of the default converter is kludgy but it works.
		converters: {
			"text script": function() {}
		},
		dataFilter: function( response ) {
			jQuery.globalEval( response, options );
		}
>>>>>>> master
	} );
};

return jQuery._evalUrl;

} );
