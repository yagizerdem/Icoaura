import $ from "jquery";

function flash({
  initialOpacity = 0.5,
  fadeOut = 300,
  duration = 600,
}: {
  initialOpacity?: number;
  fadeOut?: number;
  duration?: number;
}) {
  $(".flash")
    .show() //show the hidden div
    .animate({ opacity: initialOpacity }, duration)
    .fadeOut(fadeOut)
    .css({ opacity: 1 });
}

$(document).ready(function () {
  $(".flash").hide();
});

export { flash };
