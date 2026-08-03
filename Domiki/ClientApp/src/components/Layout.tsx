import type { ReactNode } from 'react';
import { Toaster } from '../services/toast';
import { NavMenu } from './NavMenu';
import { UpdateBanner } from './UpdateBanner';

interface LayoutProps {
    children: ReactNode;
}

export const Layout = ({ children }: LayoutProps) => {
    return (
        <div>
            <a className="skip-link" href="#main-content">К содержимому</a>
            <NavMenu />
            <main id="main-content" className="app-container">
                {children}
            </main>
            <UpdateBanner />
            <Toaster />
        </div>
    );
};
