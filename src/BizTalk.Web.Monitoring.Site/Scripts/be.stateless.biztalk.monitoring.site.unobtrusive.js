/// <reference path="jquery-1.7.2.js" />
/// <reference path="jquery-ui-1.8.19.js" />
/// <reference path="jquery.ba-bbq.js" />
/// <reference path="jquery.contextMenu.js" />

/*!
 * Copyright © 2012 - 2013 François Chabot, Yves Dierick
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

(function ($) {
	function asyncRequest(element, options) {
		var dynamicRow = $(element).closest('tr').next();
		var throbber = $('div.throbber', dynamicRow);
		$.extend(options, {
			beforeSend: function (xhr, settings) {
				dynamicRow.fadeToggle();
				var loaded = dynamicRow.data("loaded") || false;
				if (loaded !== true) {
					throbber.show();
				}
				// prevent async if loaded
				return !loaded;
			},
			complete: function (xhr, status) {
				throbber.hide();
			},
			success: function (data, status, xhr) {
				var contentType = xhr.getResponseHeader("Content-Type") || "text/html";
				if (contentType.indexOf("application/x-javascript") !== -1) {
					// jQuery already executes JavaScript for us
					return;
				}
				//var target = $('td[colspan]', dynamicRow); does not work in IE8
				var target = $('td:last-child', dynamicRow);
				$(target).html(data);
				$('.accordion', target).accordion({
					autoHeight: false,
					collapsible: true,
					navigation: true
				});
				$('.tabs', target).tabs();
				dynamicRow.data("loaded", true);
			},
			error: function (xhr, status, error) {
				var content = xhr.responseText;
				// http://msdn.microsoft.com/en-us/library/ms536652(v=vs.85).aspx
				var newDocument = window.document.open("text/html");
				newDocument.write(content);
				newDocument.close();
			}
		});
		options.data.push({ name: "X-Requested-With", value: "XMLHttpRequest" });
		$.ajax(options);
	}

	function onActionClick(action, element, position) {
		// get this cell's content as filter's value
		var filterValue = element.attr('title') || element.text();
		// get this cell's column header to get filter's property name
		var filterName = element
			.closest('table')
			.find('th:nth-child(' + (element.index() + 1) + ')')
			.attr('data-filter-name');
		var selectedValue = getSelectedText();

		var filter = new Object();
		var mergeMode = 0; // params in the params argument will override any query string params in url.
		switch (action) {
			case "reset": // TODO reset filter
				// see http://plugins.jquery.com/project/ba-jquery-bbq-plugin, http://benalman.com/projects/jquery-bbq-plugin/
				var params = $.deparam.querystring();
				for (var property in params) {
					// TODO remove all column for which there is a data-filter + data-filter-name
					var value = params[property];
					if ("-*!".indexOf(value.charAt(0)) < 0) {
						filter[property] = value;
					}
				}
				mergeMode = 2; // params argument will completely replace any query string in url
				break;
			case "exclude":
				if (!filterValue || !filterName) return;
				filter[filterName] = "!" + filterValue;
				filter.page = 1;
				break;
			case "include":
				if (!filterValue || !filterName) return;
				filter[filterName] = filterValue;
				filter.page = 1;
				break;
			case "like":
				if (!selectedValue || !filterValue || !filterName) return;
				filter[filterName] = "*" + (selectedValue || filterValue);
				filter.page = 1;
				break;
			case "unlike":
				if (!filterValue || !filterName) return;
				filter[filterName] = "-" + (selectedValue || filterValue);
				filter.page = 1;
				break;
			case "open": // TODO goto step
				window.open($(element.html()).attr('href'));
				break;
			default:
				throw action + " is not a supported action value.";
		}
		window.location.href = $.param.querystring(window.location.href, filter, mergeMode);
	}

	function getSelectedText() {
		var t = '';
		if (window.getSelection) {
			t = window.getSelection();
		} else if (document.getSelection) {
			t = document.getSelection();
		} else if (document.selection) {
			t = document.selection.createRange().text;
		}
		return t;
	}

	// grid async load
	$("a[data-ajax-grid=true]").live("click", function (evt) {
		evt.preventDefault();
		asyncRequest(this, {
			url: this.href,
			type: "GET",
			data: []
		});
	});

	// provide for filtering by cell value upon right click
	$(function () {
		// find filterable columns thanks to header cell's data-filter attribute
		$('th[data-filter=true]').each(function () {
			// then for all data cells in the matching/corresponding column of the same table
			$('tr:not(.dynamic) > td:nth-child(' + ($(this).index() + 1) + ')', $(this).closest('table'))
				// set up a ctxt menu
				.contextMenu({ menu: 'filterMenu' }, onActionClick);
		});
	});


	// prevent click for readonly checkboxes to actually fake a readonly control
	$(':checkbox[data-readonly=true]').live("click", function () {
		return false;
	});
} (jQuery));