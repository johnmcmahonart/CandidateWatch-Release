import React from 'react';
import List from '@mui/material/List';
import { ICandidateCard } from '../Interfaces/Components';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';

function CandidateCard(element: ICandidateCard) {
    return (
        //<ListItemButton onClick="">
        <Card sx={{ minWidth: '2' }}>
            
            <CardContent>
                <Typography sx={{ fontSize: 14 }}>
                    {element.fullName}
                </Typography>

                <Typography sx={{ fontSize: 14 }}>
                    {element.district}
                </Typography>

                <Typography sx={{ fontSize: 14 }}>
                    {element.party}
                </Typography>

                <Typography sx={{ fontSize: 14 }}>
                    {element.electionYear}
                </Typography>

                <CardActions>
                    <Button>More Details
                    </Button>
                </CardActions>
            </CardContent>
        </Card>

    );
}

export default CandidateCard;