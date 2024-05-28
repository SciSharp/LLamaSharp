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




const Enums = {
	SessionConnectionStatus: Object.freeze({
		Disconnected: 0,
		Loaded: 4,
		Connected: 10
	}),
	ExecutorType: Object.freeze({
		Interactive: 0,
		Instruct: 1,
		Stateless: 2
	}),
	TokenType: Object.freeze({
		Begin: 0,
		Content: 2,
		End: 4,
		Cancel: 10
	}),
	GetName: (enumType, enumKey) => {
		return Object.keys(enumType)[enumKey]
	},

	GetValue: (enumType, enumName) => {
		return enumType[enumName];
	}
};