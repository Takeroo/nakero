/**
 * @license Copyright (c) 2003-2016, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */
CKEDITOR.editorConfig = function (config) {
    config.toolbarGroups = [
		{ name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
		{ name: 'styles', groups: ['styles'] },
		{ name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi', 'paragraph'] }
    ];
    //config.extraPlugins = 'imageresponsive';
    config.height = 180;

    config.removeButtons = 'Save,NewPage,Print,Cut,Copy,Paste,PasteText,PasteFromWord,Redo,Undo,Find,SelectAll,Anchor,About,Replace,CopyFormatting,RemoveFormat,Preview,Templates,Button,ImageButton,HiddenField';
};