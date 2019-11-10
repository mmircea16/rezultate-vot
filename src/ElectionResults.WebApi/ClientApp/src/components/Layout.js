import React from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './Header/NavMenu';
import { DonateHeader } from './Header/DonateHeader';
import { Footer } from './Footer/Footer';
import IdleTimer from 'react-idle-timer';
export function Layout(props) {

    const state = {
        timeout: 1000 * 5,
        showModal: false,
        userLoggedIn: false,
        isTimedOut: false
    };

    const _onAction = (e) => {
        window.dispatchEvent(new Event("onAction", { bubbles: true, cancelable: false }));
    }

    const _onActive = (e) => {
        window.dispatchEvent(new Event("onActive", { bubbles: true, cancelable: false }));
    }
    const _onIdle = (e) => {
        window.dispatchEvent(new Event("onIdle", { bubbles: true, cancelable: false }));
    }
    let idleTimer = null;
    const onAction = _onAction.bind(this);
    const onActive = _onActive.bind(this);
    const onIdle = _onIdle.bind(this);

    return (
        <>
            <IdleTimer
                ref={ref => { idleTimer = ref }}
                element={document}
                onActive={onActive}
                onIdle={onIdle}
                onAction={onAction}
                debounce={250}
                timeout={state.timeout} />
            <div>
                <NavMenu />
                <DonateHeader />
                <Container>
                    {props.children}
                </Container>
                <Footer />
            </div>
        </>

    )
}
