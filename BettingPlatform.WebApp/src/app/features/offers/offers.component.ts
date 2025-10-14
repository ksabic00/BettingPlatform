import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OffersService, OfferDto, OfferOutcomeDto, OfferCategory } from '../../api';
import { BetslipService } from '../../core/state/betslip.service';

@Component({
  standalone: true,
  selector: 'app-offers',
  imports: [CommonModule],
  templateUrl: './offers.component.html',
  styleUrls: ['./offers.component.scss']
})
export class OffersComponent implements OnInit {
  offers: OfferDto[] = [];
  OfferCategory = OfferCategory; 

  private selectedMap = new Map<string, string>(); 

  constructor(private api: OffersService, private slip: BetslipService) {
    this.slip.selections$.subscribe(list => {
      this.selectedMap.clear();
      list.forEach(s => this.selectedMap.set(s.offerId, s.outcomeTemplateId));
    });
  }

  ngOnInit(): void {
    this.api.apiOffersActiveGet().subscribe(o => this.offers = o);
  }

  trackById = (_: number, x: OfferDto) => x.offerId;

  isSelected(offerId?: string, outcomeId?: string) {
    if (!offerId || !outcomeId) return false;
    return this.selectedMap.get(offerId) === outcomeId;
  }

  pick(o: OfferDto, oc: OfferOutcomeDto) {
    this.slip.addOrReplace(o, oc);
  }
}
