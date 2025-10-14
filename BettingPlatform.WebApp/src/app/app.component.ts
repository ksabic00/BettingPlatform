import { Component } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter, map, startWith } from 'rxjs/operators';
import { AsyncPipe, CommonModule, NgIf } from '@angular/common';
import { BetslipPanelComponent } from './features/betslip/betslip-panel/betslip-panel.component';

@Component({
  selector: 'app-root',
  standalone: true,
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  imports: [CommonModule,   RouterOutlet, RouterLink, RouterLinkActive, NgIf, AsyncPipe,
    BetslipPanelComponent]
})
export class AppComponent {
  hideAside$ = this.router.events.pipe(
    filter(e => e instanceof NavigationEnd),
    map(() => {
      let r = this.route;
      while (r.firstChild) r = r.firstChild;
      return r.snapshot.data['hideAside'] === true;
    }),
    startWith(this.route.snapshot.firstChild?.data?.['hideAside'] === true)
  );

  constructor(private router: Router, private route: ActivatedRoute) {}
}
