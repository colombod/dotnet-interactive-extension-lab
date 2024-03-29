// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import * as fluentui from '@fluentui/react';
import * as vega from 'vega';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Explorer, use, Props } from '@msrvida/sanddance-explorer';
import "@msrvida/sanddance-explorer/dist/css/sanddance-explorer.css";
import "./app.css";
import { SandDanceDataExplorerCommandHandler } from "./SandDanceDataExplorerCommandHandler";
import { Data } from "./dataTypes";

fluentui.initializeIcons();

use(fluentui, React, ReactDOM, vega);

export interface DataExplorerSettings {
    container: HTMLDivElement,
    data: Data,
    id: string
}

export function createSandDanceExplorer(settings: DataExplorerSettings) {

    let controller = new SandDanceDataExplorerCommandHandler(settings.id);
    const explorerProps: Props = {
        logoClickUrl: 'https://microsoft.github.io/SandDance/',

        mounted: explorer => {
            controller.setExplorer(explorer);
            controller.loadData(settings.data);
        }
    };

    ReactDOM.render(React.createElement(Explorer, explorerProps), settings.container);
}