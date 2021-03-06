//
//  AddTagButton.swift
//  TogglDesktop
//
//  Created by Nghia Tran on 6/13/19.
//  Copyright © 2019 Alari. All rights reserved.
//

import Cocoa
import Carbon.HIToolbox

protocol AddTagButtonDelegate: class {
    func shouldOpenTagAutoComplete(with text: String)
}

final class AddTagButton: NSButton {

    // MARK: Variables

    weak var delegate: AddTagButtonDelegate?
    private let ignoreKeys = [kVK_Tab,kVK_Space] // Tab and space

    // MARK: Override

    override func keyDown(with event: NSEvent) {
        super.keyDown(with: event)

        // Open the auto complete if the key isn't ignore key
        guard let characters = event.characters, !ignoreKeys.contains(Int(event.keyCode)) else { return }

        // Replace "enter" with empty string if need
        let text = characters == "\r" ? "" : characters

        // Notify
        delegate?.shouldOpenTagAutoComplete(with: text)
    }
}
