

$(function () {

    'use strict';

    var console = window.console || { log: function () { } };
    var $body = $('body');

    // Demo
    // ---------------------------------------------------------------------------

    (function () {
        var $image = $('.img-container > img');
        var $actions = $('.img-operation-btns');
        var options = {
            aspectRatio: 640 / 364,
            minCropBoxWidth: 640,
            minCropBoxHeight: 364,
            cropBoxResizable: false,
            autoCropArea: 0.3,
            cropBoxMovable: false,
            dragCrop: false,
            strict: false,
        };

        $image.eq(0).cropper(options);


        // Buttons
        if (!$.isFunction(document.createElement('canvas').getContext)) {
            $('button[data-method="getCroppedCanvas"]').prop('disabled', true);
        }

        if (typeof document.createElement('cropper').style.transition === 'undefined') {
            $('button[data-method="rotate"]').prop('disabled', true);
            $('button[data-method="scale"]').prop('disabled', true);
        }

        // Options
        $actions.on('change', ':checkbox', function () {

            var $this = $(this);
            var cropBoxData;
            var canvasData;

            if (!$image.data('cropper')) {
                return;
            }

            options[$this.val()] = $this.prop('checked');

            cropBoxData = $image.cropper('getCropBoxData');
            canvasData = $image.cropper('getCanvasData');
            options.built = function () {
                $image.cropper('setCropBoxData', cropBoxData);
                $image.cropper('setCanvasData', canvasData);
            };

            $image.cropper('destroy').cropper(options);
        });


        // Methods
        $actions.on('click', '[data-method]', function () {

            var $this = $(this);
            var data = $this.data();
            var $target;
            var result;
            var yuanshitupian;
            var $nowimage = $image.eq($this.parents('.img-operation-btns').eq(0).data('index') - 1);

            $(".cropper-canvas").find("img");

            if ($this.prop('disabled') || $this.hasClass('disabled')) {
                return;
            }

            if ($nowimage.data('cropper') && data.method) {
                data = $.extend({}, data); // Clone a new one

                if (typeof data.target !== 'undefined') {
                    $target = $(data.target);

                    if (typeof data.option === 'undefined') {
                        try {
                            data.option = JSON.parse($target.val());
                        } catch (e) {
                            console.log(e.message);
                        }
                    }
                }
                data.option = data.method === 'getCroppedCanvas' ? { width: 640, height: 364 } : undefined;
                result = $nowimage.cropper(data.method, data.option, data.secondOption);


                switch (data.method) {
                    case 'scaleX':
                    case 'scaleY':
                        $(this).data('option', -data.option);
                        break;
                    case 'getCroppedCanvas':
                        if (result) {

                            $("#enlogimage").val(result.toDataURL());
                            $this.parents('.img-operation-btns').parent().css("height", "0px");

                            onsubmitformimage();
                          
                            $nowimage.cropper('replace', "/Images/basicInfo_img.png");
                        }
                        break;
                    case 'zoom':
                        $nowimage.cropper("zoom", $this.data("option"));
                        break;
                }

                if ($.isPlainObject(result) && $target) {
                    try {
                        $target.val(JSON.stringify(result));
                    } catch (e) {
                        console.log(e.message);
                    }
                }

            }
        });


        // Keyboard
        $body.on('keydown', function (e) {

            if (!$image.data('cropper') || this.scrollTop > 300) {
                return;
            }

            switch (e.which) {
                case 37:
                    e.preventDefault();
                    $image.cropper('move', -1, 0);
                    break;

                case 38:
                    e.preventDefault();
                    $image.cropper('move', 0, -1);
                    break;

                case 39:
                    e.preventDefault();
                    $image.cropper('move', 1, 0);
                    break;

                case 40:
                    e.preventDefault();
                    $image.cropper('move', 0, 1);
                    break;
            }

        });

        // Import image
        var $inputImage = $('.inputImage');
        var URL = window.URL || window.webkitURL;
        var blobURL;

        if (URL) {

            $inputImage.change(function () {
                var $this = $(this);
                var files = this.files;
                var file;
                var $nowimage = $image.eq($this.parents('.img-operation-btns').eq(0).data('index') - 1);

                if (!$nowimage.data('cropper')) {
                    return;
                }

                if (files && files.length) {
                    file = files[0];


                    if (/^image\/\w+$/.test(file.type)) {

                        blobURL = URL.createObjectURL(file);

                        $nowimage.one('built.cropper', function () {

                            URL.revokeObjectURL(blobURL); // Revoke when load complete
                        }).cropper('reset').cropper('replace', blobURL);

                        //var reader = new FileReader();//新建一个FileReader

                        //reader.readAsDataURL(file);
                        //reader.onload = function (evt) { //读取完文件之后会回来这里
                        //    var fileString = evt.target.result;
                        //    $("#originalimage").val(fileString); //原始图片Base64
                        //}

                        $this.val('');

                    } else {
                        alert('Please choose an image file.');
                    }
                }
            });
        } else {
            $inputImage.prop('disabled', true).parent().addClass('disabled');
        }

    }());

});

function getPath(obj) {
    if (obj) {
        if (window.navigator.userAgent.indexOf("MSIE") >= 1) {
            obj.select();
            return document.selection.createRange().text;
        } else if (window.navigator.userAgent.indexOf("Firefox") >= 1) {
            if (obj.files) {
                return obj.files.item(0).getAsDataURL();
            }
            return obj.value;
        }
        return obj.value;
    }
}


