let _requestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
$.ajaxPrefilter(function (options, originalOptions) {
	options.async = true;
	if (options.type.toUpperCase() == "POST") {
		options.data = $.param($.extend(originalOptions.data, { __RequestVerificationToken: _requestVerificationToken }));
	}
});
$.ajaxSetup({ cache: false });


const ajaxPostJsonAsync = (url, data) => {
	return $.ajax({
		url: url,
		cache: false,
		async: true,
		type: "POST",
		dataType: 'json',
		data: data
	});
}


const ajaxGetJsonAsync = (url, data) => {
	return $.ajax({
		url: url,
		cache: false,
		async: true,
		type: "GET",
		dataType: 'json',
		data: data
	});
}