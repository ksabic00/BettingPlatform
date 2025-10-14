import { Routes } from '@angular/router';
import { OffersComponent } from './features/offers/offers.component';
import { BetslipComponent } from './features/betslip/betslip.component';
import { WalletComponent } from './features/wallet/wallet.component';
import { TicketsComponent } from './features/tickets/tickets.component';

export const routes: Routes = [
  { path: '', redirectTo: 'offers', pathMatch: 'full' },
  { path: 'offers', component: OffersComponent },
  { path: 'betslip', component: BetslipComponent, data: { hideAside: true } }, 
  { path: 'wallet', component: WalletComponent },
  { path: 'tickets', component: TicketsComponent },
  { path: '**', redirectTo: 'offers' }
];
