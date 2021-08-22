// viewport size
function viewport() {
	var a = window,
		b = "inner";
	return (
		"innerWidth" in window ||
			((b = "client"), (a = document.documentElement || document.body)),
		{ width: a[b + "Width"], height: a[b + "Height"] }
	);
}
// viewport size end

// clear timeout
function timerOut(clearTimer) {
	clearTimeout(clearTimer);
}
// clear timeout end

// cols
function fixEqualHeight() {
	$(".js-col").css("height", "auto");
	$(".js-cols").each(function () {
		var maxHeight = -1;
		$(this)
			.find(".js-col")
			.each(function () {
				maxHeight =
					maxHeight > $(this).height() ? maxHeight : $(this).height();
			});
		$(this)
			.find(".js-col")
			.each(function () {
				$(this).css("height", maxHeight + "px");
			});
	});
}
// cols end

// load document
window.onload = function () {
	// ie fix
	if (/MSIE 10/i.test(navigator.userAgent)) {
		$("html").addClass("ie");
	}
	if (
		/MSIE 9/i.test(navigator.userAgent) ||
		/rv:11.0/i.test(navigator.userAgent)
	) {
		$("html").addClass("ie");
	}
	// ie fix

	// remove loaded class
	document.body.classList.remove("loaded");
	// remove loaded class end

	// check device type
	if (
		/Android|Windows Phone|webOS|iPhone|iPad|iPod|BlackBerry/i.test(
			navigator.userAgent
		)
	) {
		document.documentElement.classList.add("mob");
	} else {
		document.documentElement.classList.add("desktop");
	}

	if (navigator.platform.toUpperCase().indexOf("MAC") >= 0) {
		document.documentElement.classList.add("mac");
	}
	// check device type end

	// placeholder
	$(function () {
		$("input, textarea").each(function () {
			var a = $(this).attr("placeholder");
			$(this).focus(function () {
				$(this).attr("placeholder", "");
			});
			$(this).focusout(function () {
				$(this).attr("placeholder", a);
			});
		});
	});
	// placeholder end

	// lazy load
	(function () {
		var srcArr = document.querySelectorAll("[data-lazy]");
		if($(".mob").length){
			setTimeout(function () {
				for (var i = 0; i < srcArr.length; i++) {
					var newSrc = srcArr[i].dataset.lazy;
					srcArr[i].src = newSrc;
				}
			}, 1500);
		} else{
			for (var i = 0; i < srcArr.length; i++) {
				var newSrc = srcArr[i].dataset.lazy;
				srcArr[i].src = newSrc;
			}
		}		
	})();

	(function () {
		var srcArr = document.querySelectorAll("[data-bg]");
		if($(".mob").length){
			setTimeout(function () {
				for (var i = 0; i < srcArr.length; i++) {
					var newSrc = srcArr[i].dataset.bg;
					srcArr[i].style.backgroundImage = "url(" + newSrc + ")";
				}
			}, 1500);
		} else{
			for (var i = 0; i < srcArr.length; i++) {
				var newSrc = srcArr[i].dataset.bg;
				srcArr[i].style.backgroundImage = "url(" + newSrc + ")";
			}
		}
	})();
	// lazy load end

	// popups
	(function () {
		var timer;

		// open popup
		jQuery(".js-popup-open").on("click", function (e) {

			e.preventDefault();
			var currentId =
				jQuery(this).attr("href") || jQuery(this).attr("data-href");
			timerOut(timer);

			// append responsive video in popup
			if (jQuery(this).hasClass("js-popup-video")) {
				var videoSrc = jQuery(this).attr("data-frame");
				var videoWidth = jQuery(this).attr("data-width");
				var videoHeight = jQuery(this).attr("data-height");
				jQuery(".js-video-frame").append(
					'<iframe frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>'
				);
				jQuery(".js-video-frame")
					.find("iframe")
					.attr("src", videoSrc)
					.attr("width", videoWidth)
					.attr("height", videoHeight);
				jQuery(".js-video-frame").fitVids().addClass("active");
			}			

			if (document.body.scrollHeight > document.body.clientHeight) {
				jQuery("html").addClass("remove-scroll");
			}
			jQuery(currentId).stop().fadeIn(0).addClass("active");
			jQuery("html").addClass("hidden");
			if (jQuery(currentId).scrollTop() > 0) {
				jQuery(currentId).scrollTop(0);
			}
			return false;

		});

		// close popup
		jQuery(".js-popup-close").on("click", function (e) {

			if (jQuery(this).is("a")) {
				e.preventDefault();
			}
			timerOut(timer);
			jQuery(".js-popup.active").removeClass("active").fadeOut(300);
			timer = setTimeout(function () {
				if (!$(".js-mob-hide.active").length) {
					jQuery("html").removeClass("hidden remove-scroll");
				}

				// clear responsive video in popup
				if (jQuery(".js-video-frame.active").length) {
					jQuery(".js-video-frame").empty().removeClass("active");
				}	

			}, 300);

		});
	})();
	// popups

	// closest descendent
	$.fn.closestDescendent = function (filter) {
		var $found = $(),
			$currentSet = this;
		while ($currentSet.length) {
			$found = $currentSet.filter(filter);
			if ($found.length) break;
			$currentSet = $currentSet.children();
		}
		return $found.first();
	};
	// closest descendent end

	// navigation
	$(function () {
		var navTimer;

		$(".js-mob-open").live("click", function () {
			timerOut(navTimer);
			$(".js-mob-hide")
				.stop()
				.fadeIn(100, function () {
					$(this).addClass("active");
				});
			if (document.body.scrollHeight > document.body.clientHeight) {
				$("html").addClass("remove-scroll");
			}
			$("html").addClass("hidden");
		});

		$(".js-mob-close").live("click", function () {
			timerOut(navTimer);
			$(".js-mob-hide")
				.stop()
				.fadeOut(100, function () {
					$(this).removeClass("active");
				});
			navTimer = setTimeout(function () {
				$("html").removeClass("hidden remove-scroll");
			}, 300);
		});

	});
	// navigation end

	// sliders
	if ($(".js-slider").length) {
		$(function () {

			$(".js-slider-item").each(function(){
				$(this).attr("data-item",$(this).index());
			});	

			$(".js-slider-item[data-item=0]").addClass("active-1");
			$(".js-slider-item[data-item=1]").addClass("active-2");
			$(".js-slider-item[data-item=2]").addClass("active-3");
			$(".js-slider-item[data-item=3]").addClass("active-4");
			$(".js-slider-item[data-item=4]").addClass("active-5");				

			function sliderInit(slider, arrows) {
				slider.slick({
					dots: false,
					arrows: true,
					infinite: true,
					autoplay: false,
					slidesToShow: 5,
					slidesToScroll: 1,
					touchThreshold: 200,
					speed: 500,
					swipeToSlide: true,
					center:true,
					appendArrows:arrows
				});
			}

			function newsInit(slider, arrows) {
				slider.slick({
					dots: false,
					arrows: true,
					infinite: true,
					autoplay: false,
					slidesToShow: 3,
					slidesToScroll: 1,
					touchThreshold: 200,
					speed: 500,
					swipeToSlide: true,
					center:true,
					appendArrows:arrows,
					adaptiveHeight: true,
					responsive: [
						{ breakpoint: 1101, settings: { slidesToShow: 2 } },
						{ breakpoint: 701, settings: { slidesToShow: 1 } }
					],
				});
			}			

			setTimeout(function(){
				$(".js-slider-list").on("init", function () {
					$(".js-slider-loader").removeClass("loaded");
				});

				$(".js-slider-list").each(function () {
					var $this = $(this),
						$arrows = $this
							.closest(".js-slider")
							.find(".js-slider-arrows");
					if($(this).hasClass("js-slider-news")){
						newsInit($this, $arrows);
					} else{
						sliderInit($this, $arrows);					
					}
				});		
	
				$(".js-slider-list").on("beforeChange", function (
					event,
					slick,
					currentSlide,
					nextSlide
				) {
					$(this).find(".js-slider-item").removeClass("active-1 active-2 active-3 active-4 active-5");
					$(this).find(".js-slider-item[data-item="+nextSlide+"]").addClass("active-1").removeClass("active-2 active-3 active-4 active-5");
					var slickSlideNext1 = $(this).find(".js-slider-item[data-item="+nextSlide+"]").closest(".slick-slide").next(".slick-slide").find(".js-slider-item").attr("data-item");
					var slickSlideNext2 = $(this).find(".js-slider-item[data-item="+nextSlide+"]").closest(".slick-slide").next(".slick-slide").next(".slick-slide").find(".js-slider-item").attr("data-item");	
					var slickSlideNext3 = $(this).find(".js-slider-item[data-item="+nextSlide+"]").closest(".slick-slide").next(".slick-slide").next(".slick-slide").next(".slick-slide").find(".js-slider-item").attr("data-item");	
					var slickSlideNext4 = $(this).find(".js-slider-item[data-item="+nextSlide+"]").closest(".slick-slide").next(".slick-slide").next(".slick-slide").next(".slick-slide").next(".slick-slide").find(".js-slider-item").attr("data-item");
					$(this).find(".js-slider-item[data-item="+ slickSlideNext2+"]").addClass("active-3");
					$(this).find(".js-slider-item[data-item="+ slickSlideNext1+"]").addClass("active-2");
					$(this).find(".js-slider-item[data-item="+ slickSlideNext3+"]").addClass("active-4");
					$(this).find(".js-slider-item[data-item="+ slickSlideNext4+"]").addClass("active-5");
				});
			}, 2000);

		});
	}
	// sliders end

	// accordion
	$(function () {
		$(".js-faq-item.active").find(".js-faq-hide").slideDown(0);

		$(".js-faq-button").on("click", function () {
			if ($(this).closest(".js-faq-item").hasClass("active")) {
				$(this)
					.closest(".js-faq-item")
					.find(".js-faq-hide")
					.slideUp(300);
				$(this)
					.closest(".js-faq")
					.find(".js-faq-item")
					.removeClass("active");
			} else {
				$(this)
					.closest(".js-faq")
					.find(".js-faq-item.active .js-faq-hide")
					.slideUp(300);
				$(this)
					.closest(".js-faq")
					.find(".js-faq-item.active")
					.removeClass("active");
				$(this).closest(".js-faq-item").addClass("active");
				$(this)
					.closest(".js-faq-item")
					.closestDescendent(".js-faq-hide")
					.slideDown(300, function () {
						if($(".mob").length){
							var url = $(this).closest(".js-faq-item");
							$("html, body").animate(
								{
									scrollTop: parseInt($(url).offset().top),
								},
								300
							);
						}
					});
			}
		});
	});
	// accordion
	
	// scroll to id
	(function () {
		jQuery(".js-scrolltoid").on("click", function (e) {
			e.preventDefault();
			var url = jQuery(this).attr("href");
			jQuery("html, body").animate(
				{
					scrollTop: parseInt(jQuery(url).offset().top),
				},
				700
			);
			if($(".js-mob-hide").hasClass("active")){
				$(".js-mob-close:first").trigger("click");
			}
		});
	})();
	// scroll to id	

	// multiline truncating
	$(function () {
		if ($(".js-ellip-2").length) {
			var ellipsis = window.ellipsed.ellipsis;
			ellipsis(".js-ellip-2", 2);
		}
	});	
	// multilin truncating end

	// datepicker
	if ($(".js-datepicker").length) {

		function fixDatepicker(){
			$(".js-datepicker").each(function(){
				$(this).find("td[data-handler]:first").addClass("radius-top-left");
				$(this).find("tr:first-child td[data-handler]:last-child").addClass("radius-top-right");
				if(!$(this).find("td:first-child").hasClass("radius-top-left")){
					$(this).find("tr:nth-child(2) td:first-child").addClass("radius-top-left");
				}
				$(this).find("td[data-handler]:last").addClass("radius-bottom-right");
				if(!$(this).find("tr:last-child td:last-child").hasClass("radius-bottom-right")){
					$(this).find("tr:nth-last-child(2) td:last-child").addClass("radius-bottom-right");
				}				
			});
		}

		$(".js-datepicker").datepicker({
			focusOnShow: false,
			ignoreReadonly: true,
			inline: true,
			showAnim: "",
			prevText: "",
			nextText: "",
			dayNamesMin: ["M", "T", "W", "S", "F", "S", "S"],
			onSelect: function () {
				setTimeout(function(){
					fixDatepicker();
				}, 1);
			},
			onChangeMonthYear: function () {
				setTimeout(function(){
					fixDatepicker();
				}, 1);
			},				
		});
		setTimeout(function(){
			fixDatepicker();
		}, 100);
	}
	// datepicker end	
};
// load document end

// resize window
function resizeWindow() {

	// moving content
	if (viewport().width > 900) {
		$(".js-content-1").appendTo(".js-from-1");
	}
	if (viewport().width <= 900) {
		$(".js-content-1").appendTo(".js-to-1");
	}
	if (viewport().width > 479) {
		$(".js-content-2").appendTo(".js-from-2");
	}
	if (viewport().width <= 479) {
		$(".js-content-2").appendTo(".js-to-2");
	}
	// moving content

	// equal items fix
	fixEqualHeight();
	setTimeout(function () {
		fixEqualHeight();
		if ($(".js-slider-list.js-cols .slick-list").length) {
			$(".js-slider-list.js-cols").slick("setPosition");
		}
	}, 800);
	// equal items fix end	

}

window.addEventListener("load", resizeWindow);
window.addEventListener("resize", resizeWindow);
window.addEventListener("oriantationchange", resizeWindow);
// resize window end


