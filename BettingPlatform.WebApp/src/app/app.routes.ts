import { Routes } from '@angular/router';
import { OffersComponent } from './features/offers/offers.component';
import { BetslipComponent } from './features/betslip/betslip.component';
import { WalletComponent } from './features/wallet/wallet.component';
import { TicketsComponent } from './features/tickets/tickets.component';
import { PlayerComponent } from './features/player/player.component';
import { TicketDetailComponent } from './features/tickets/ticket-detail/ticket-detail.component';

export const routes: Routes = [
  { path: '', redirectTo: 'players', pathMatch: 'full' },
  { path: 'players', component: PlayerComponent, data: { hideAside: true } },
  { path: 'offers', component: OffersComponent },
  { path: 'betslip', component: BetslipComponent, data: { hideAside: true } }, 
  { path: 'wallet', component: WalletComponent },
  { path: 'tickets', component: TicketsComponent },
  { path: 'tickets/:id', component: TicketDetailComponent }
];
