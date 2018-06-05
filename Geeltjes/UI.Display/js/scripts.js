(function ($) {


    $.icatt = $.extend($.icatt, {

        stickynotes: function (options) {

            var defaultSettings = {
                ajaxPath: null
                , portalId: null
                , editEnabled: false
                , modulePath: '/DesktopModules/Icatt/Geeltjes/UI.Display/'
                , clientId: null
                , moduleId: null
                , userFullName: null
                , editTitle: 'Wijzigen notitie'
                , addTitle: 'Notitie toevoegen'
                , addButtonTitle: 'Notitie toevoegen'
                , editButtonTitle: 'Wijzigingen bewaren'
                , confirmDeleteText: "Are you sure you want to delete note\n\n'{0}'\n\nfrom '{1}'?" //{0} == bodytext {1} == author
                , tooShortText: "The note text is too short!"
                , noAuthorText: "You haven't entered your name!"
                , classNames: {
                    addButton: 'add-button'
                    , saveButton: 'save-button'
                }
            };

            var settings = $.extend(true, defaultSettings, options);

            var path = settings.ajaxPath;
            var clientId = settings.clientId;

            var $outerContainer = $('#' + clientId);
            var $notesContainer = $outerContainer.find('#' + clientId + '_notesContainer');
            var $notes = $notesContainer.find('.note');
            var $addButton = $notesContainer.find('a.' + settings.classNames.addButton);
            var $noteEditor = $('#' + clientId + '_noteEditor');
            var $noteEditorTitle = $noteEditor.find('.popupTitle');
            var $noteEditorErrorMessage = null;
            var $noteEditorColorButtons = $noteEditor.find('.color');
            var $noteEditorNoteId = $noteEditor.find('span.data');
            var $noteEditorInput = $noteEditor.find('.pr-body,.pr-author');
            var $noteEditorInputAuthor = $noteEditor.find('.pr-author');
            var $noteEditorInputBody = $noteEditor.find('.pr-body');
            var $notePreview = $noteEditor.children('.note');
            var $noteEditorSaveButton = $noteEditor.find('a.' + settings.classNames.saveButton);
            var $notePreviewBody = $notePreview.children('.body');
            var $notePreviewAuthor = $notePreview.children('.author');
            var $editButtons = $notes.find("a.control.edit");
            var $deleteButtons = $notes.find('a.control.delete');
            var $thumbsup = $notes.find('a.thumbs.up');
            var $thumbsdown = $notes.find('a.thumbs.down');
            var zIndex = 0;

            var editFancyboxOptions = {
                afterShow: function (instance, slide) {

                    var btn = $(slide.opts.$orig[0]);

                    var $note = btn.closest('.note'),
                        noteAuthor = $note.find('.author').text().trim(),
                        noteBody = $note.find('.body').text().trim(),
                        noteColor = $note.hasClass('green') ? 'green' : ($note.hasClass('blue') ? 'blue' : 'yellow'),
                        noteId = $note.find('.data').text();

                    //
                    // Set editor title and save button texts

                    $noteEditorTitle.html(settings.edittitle);
                    $noteEditorSaveButton.text(settings.editbuttontitle);

                    //
                    // Set field values

                    $noteEditorInputAuthor.val(noteAuthor);
                    $noteEditorInputBody.val(noteBody.replace(/<br[^>]*>/ig, "\n"));

                    //
                    // Set note preview texts

                    $notePreviewAuthor.html(noteAuthor);
                    $notePreviewBody.html(noteBody);
                    $notePreview.removeClass('yellow green blue').addClass(noteColor);
                    $noteEditorNoteId.text('' + noteId);

                    if ($noteEditorErrorMessage != null)
                        $noteEditorErrorMessage.remove();


                }
            };

            $notes.each(function () {
                /* Finding the biggest z-index value of the notes */
                var tmp = parseInt($(this).css('z-index'));
                if (!isNaN(tmp) && tmp > zIndex) zIndex = tmp;
            });

            makeDraggable($notes);

            //noteEditor add window
            $addButton.fancybox({
                beforeShow: function (sender, index, fancyboxOptions) {
                    var $sender = $(sender);

                    $noteEditorTitle.html(settings.addTitle);
                    $noteEditorSaveButton.text(settings.addButtonTitle);

                    if ($noteEditorErrorMessage != null)
                        $noteEditorErrorMessage.remove();

                    //prefill note editor author
                    $noteEditorInputAuthor.val(settings.userFullName);
                    $notePreviewAuthor.html(settings.userFullName);

                    $noteEditorInputBody.val('');
                    $notePreviewBody.html('');
                    $notePreview.removeClass('yellow green blue').addClass('yellow');
                    $noteEditorNoteId.text('');

                    $noteEditor.data('fancybox', $sender);

                }
            });

            /* Listening for keyup events on fields of the "Add a note" form: */
            $(document).on('keyup', $noteEditorInput.selector, function (e) {

                var $this = $(this);

                /* Setting the text of the preview to the contents of the input field, and stripping all the HTML tags: */
                if ($this.hasClass("pr-author")) {
                    $notePreviewAuthor.html($this.val().replace(/<[^>]+>/ig, ''));
                }

                if ($this.hasClass("pr-body")) {
                    var htm = $this.val().replace(/<[^>]+>/ig, '').replace(/\n/g, '<br />');
                    $notePreviewBody.html(htm);
                }


            });

            /* Changing the color of the preview note: */
            $(document).on('click', $noteEditorColorButtons.selector, function () {
                var $this = $(this);
                $notePreview.removeClass('yellow green blue').addClass($this.attr('class').replace('color', ''));
            });

            /* Delete the notes */
            $(document).on('click', $deleteButtons.selector, function () {

                var $note = $(this).closest(".note");
                var geeltjeId = parseInt($note.find('span.data').text());

                var confirmDelete = settings.confirmDeleteText.replace("{0}", $note.find('.body').text()).replace("{1}", $note.find('.author').text().trim());
                var r = confirm(confirmDelete);
                if (r == true) {
                    processNotes(geeltjeId, "delete", settings.moduleId);
                    $note.remove();
                }
            });

            //noteEditor edit window
            $editButtons.fancybox(editFancyboxOptions);


            /* Rate the notes */
            $(document).on('click', $thumbsup.selector, function () {
                var $upBtn = $(this);
                var $note = $upBtn.closest(".note");
                var $downBtn = $note.find('a.thumbs.down');
                var $userVoteData = $note.find('span.voted');
                var geeltjeId = parseInt($note.find('span.data').text());
                var userVote = parseInt($note.find('span.voted').text());
                if (userVote < 1) {
                    //update
                    processNotes(geeltjeId, "thumbsup", settings.moduleId);
                    var $upSpan = $note.find('span.total-up');
                    var upValue = $upSpan.text();
                    var upCount = parseInt(upValue, 10) + 1;
                    $upSpan.text('' + upCount);

                    if (userVote < 0) {
                        //has already voted down, change
                        var $downSpan = $note.find('span.total-down');
                        var downValue = $downSpan.text();
                        var downCount = parseInt(downValue, 10) - 1;
                        $downSpan.text('' + downCount);
                    }
                }
                $userVoteData.text('1');

                //enable downbutton
                $upBtn.addClass('disabled');
                $downBtn.removeClass('disabled');
            });

            $(document).on('click', $thumbsdown.selector, function () {
                var $downBtn = $(this);
                var $note = $downBtn.closest(".note");
                var $upBtn = $note.find('a.thumbs.up');
                var geeltjeId = parseInt($note.find('span.data').text());
                var $userVoteData = $note.find('span.voted');
                var userVote = parseInt($userVoteData.text());
                if (userVote > -1) {
                    //has not voted or voted up, update vote
                    processNotes(geeltjeId, "thumbsdown", settings.moduleId);

                    //increase down coutner
                    var $downSpan = $note.find('span.total-down');
                    var downValue = $downSpan.text();
                    var downCount = parseInt(downValue, 10) + 1;
                    $downSpan.text('' + downCount);

                    if (userVote > 0) {
                        //has already voted up, change up counter
                        var $upSpan = $note.find('span.total-up');
                        var upValue = $upSpan.text();
                        var upCount = parseInt(upValue, 10) - 1;
                        $upSpan.text('' + upCount);
                    }

                    //update userVote data
                    $userVoteData.text('-1');

                    //enable upBtn
                    $downBtn.addClass('disabled');
                    $upBtn.removeClass('disabled');

                }

            });

            /* The submit button: */
            $(document).on('click', $noteEditorSaveButton.selector, function (e) {

                e.preventDefault();


                if ($('.pr-body').val().length < 4) {
                    alert(settings.tooShortText);
                    return false;
                }

                if ($('.pr-author').val().length < 1) {
                    alert(settings.noAuthorText);
                    return false;
                }

                var $this = $(this);
                var $parent = $this.parent();
                var loadingHtml = '<img class="loaderImage" src="' + settings.modulePath + 'img/ajax-loader.gif" style="margin:30px auto;display:block" />';
                var $saveBtn = $this;

                if ($noteEditorNoteId.text() == '') {
                    //new note

                    addNote($this, $parent, $saveBtn, loadingHtml);


                } else {

                    var noteId = parseInt($noteEditorNoteId.text());

                    updateNote(noteId, $this, $parent, $saveBtn, loadingHtml);

                }; //end update note

                $.fancybox.close();


                return false;
            });



            //////////////////////
            //private methods 
            //////////////////////
            function updateNote(noteId, $this, $parent, $saveBtn, loadingHtml) {
                var data = {
                    'edit': 'edit',
                    'id': noteId,
                    'moduleId': settings.moduleId,
                    'body': $noteEditor.find('.pr-body').val().replace(/\n/g, '<br />'),
                    'author': $noteEditor.find('.pr-author').val(),
                    'color': $.trim($notePreview.attr('class').replace('note', ''))
                };

                /* Sending an AJAX POST request: */
                $.ajax({
                    async: true
                    , type: "POST"
                    //					, portalId: settings.portalId
                    , url: path
                    , data: data
                    , cache: false
                    , beforeSend: function () {
                        $this.replaceWith(loadingHtml);
                    }
                }).done(function (returnData) {

                    var returnValue = new String(returnData);
                    if (returnValue.substr(0, 5) == "Error") {
                        $noteEditorErrorMessage = $parent.append('<div class="error">Opslaan mislukt. Probeer het opnieuw. Waarschuw bij herhaald falen de webmaster.</div>');
                    } else {

                        //find the note
                        //update it
                        var $note = $notes.filter(function (index) {
                            var $nodeIdSpan = $(this).find('span.data');
                            var filterId = parseInt($nodeIdSpan.text());
                            if (filterId == noteId) return true;

                            return false;

                        });

                        if ($note.length > 0) {
                            $note.find('.body').html(data.body);
                            $note.find('.author').html(data.author);
                            $note.removeClass('yellow green blue').addClass(data.color);
                        }

                    }
                }).fail(function (returnData) {
                    $noteEditorErrorMessage = $parent.append('<div class="error">Opslaan mislukt. De server is niet bereikbaar.</div>');
                }).always(function (returnData) {
                    $parent.find("img.loaderImage").replaceWith($saveBtn);
                });


            }


            function addNote($this, $parent, $saveBtn, loadingHtml) {


                //calculate position for new note
                //find dimensions notesContainer
                var containerH = $notesContainer.height();
                var containerW = $notesContainer.width();

                //exact middle notescontainer
                var middleH = Math.round(containerH / 2);
                var middleW = Math.round(containerW / 2);

                //dimensions note
                var noteH = $notePreview.height();
                var noteW = $notePreview.width();

                //left corner of note in excat middle
                middleH = middleH - Math.round(noteH / 2);
                middleW = middleW - Math.round(noteW / 2);

                //random spread 1 note width/height
                var spread = 1;
                var randomH = Math.round(noteH * spread * (Math.random() - 0.5));
                var randomW = Math.round(noteW * spread * (Math.random() - 0.5));

                var top = middleH + randomH;
                var left = middleW + randomW;

                //note must remain within boundaries container
                top = Math.max(top, 0);
                top = Math.min(top, containerH - noteH);
                left = Math.max(left, 0);
                left = Math.min(left, containerW - noteW);

                var data = {
                    'zindex': ++zIndex,
                    //					'portalId' : settings.portalId ,
                    'x': left,
                    'y': top,
                    'moduleId': settings.moduleId,
                    'body': $noteEditor.find('.pr-body').val(),
                    'author': $noteEditor.find('.pr-author').val(),
                    'color': $.trim($notePreview.attr('class').replace('note', ''))
                };


                /* Sending an AJAX POST request: */
                $.ajax({
                    async: true
                    , type: "POST"
                    , url: path
                    , data: data
                    , cache: false
                    , beforeSend: function () {
                        $this.replaceWith(loadingHtml);
                    }
                }).done(function (returnData) {

                    var returnValue = new String(returnData);
                    if (returnValue.substr(0, 5) == "Error") {
                        $noteEditorErrorMessage = $parent.append('<div class="error">Opslaan mislukt door een fout op de server. Probeer het opnieuw. Waarschuw bij herhaald falen de webmaster.</div>');
                    } else {
                        if (parseInt(returnData)) {
                            /* returnData contains the ID of the note, assigned by MySQL's auto increment: */

                            var $tmp1 = $notePreview.clone();


                            $tmp1.find('span.data').text(returnData).end().css({ 'z-index': zIndex, top: top + 'px', left: left + 'px' });
                            $tmp1.appendTo($notesContainer);

                            //add to the notes collections
                            $notes = $notes.add($tmp1);

                            makeDraggable($tmp1);

                            //activate edit button

                            $tmp1.find('a.control.edit').fancybox(editFancyboxOptions);

                        }

                    }
                }).fail(function () {
                    $noteEditorErrorMessage = $parent.append('<div class="error">Opslaan mislukt. De server is niet bereikbaar.</div>');
                }).always(function () {
                    $parent.find("img.loaderImage").replaceWith($saveBtn);
                });

            }


            function makeDraggable($elements) {
                /* $elements is a jquery object: */

                if (settings.editEnabled) {
                    $elements.draggable({
                        containment: 'parent',
                        start: function (e, ui) {
                            ui.helper.css('z-index', ++zIndex);
                        },
                        stop: function (e, ui) {
                            /* Sending the z-index and positon of the note to update_position.php via AJAX GET: */
                            var myId = parseInt(ui.helper.find('span.data').html());
                            $.get(path, {
                                x: ui.position.left,
                                y: ui.position.top,
                                z: zIndex,
                                id: myId
                            });
                        }
                    });
                    //make cursor show dragging
                    $elements.addClass("draggable");
                }
            }

            function processNotes(geeltjeId, action, moduleId) {
                $.get(path, {
                    edit: action,
                    id: geeltjeId,
                    moduleId: moduleId
                });
            }


        } //end icatt.stickynotes

    }); //end extend of $.fn.icatt object


})(jQuery);
